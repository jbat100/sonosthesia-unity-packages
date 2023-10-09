using System;
using System.Collections.Concurrent;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using MessagePack;
using Sonosthesia.Utils;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class AddressedEnvelope
    {
        [Key("address")]
        public string Address { get; set; }

        [Key("content")]
        public byte[] Content { get; set; }
    }

    [MessagePackObject]
    public class EnvelopeBundle
    {
        internal const string ADDRESS = "/bundle";
        
        [Key("envelopes")]
        public AddressedEnvelope[] Envelopes { get; set; }
    }

#region Outgoing Queue
    
    internal static class AddressedEnvelopeUtils
    {
        public static byte[] SerializedEnvelope<T>(string address, T content)
        {
            byte[] bytes = MessagePackSerializer.Serialize(content);
            AddressedEnvelope envelope = new AddressedEnvelope()
            {
                Address = address,
                Content = bytes
            };
            return MessagePackSerializer.Serialize(envelope);
        }
    }

    internal interface IOutgoingAddressedPackQueue : IDisposable
    {
        void Push<T>(string address, T content);
    }
    
    // TODO : performance stress check
    
    // note the Forget() approach might lead to messages being sent out of order
    // we could have a Queue content API which is synchronous and an internal thread which 
    // handles the sending asynchronously
    
    internal class ForgetOutgoingAddressedPackQueue : IOutgoingAddressedPackQueue
    {
        private bool _isDisposed;
        private AddressedPackConnection _connection;
        
        public ForgetOutgoingAddressedPackQueue(AddressedPackConnection connection)
        {
            _connection = connection;
        }
        
        public void Push<T>(string address, T content)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _connection.Send(AddressedEnvelopeUtils.SerializedEnvelope(address, content)).Forget();
        }
        
        public void Dispose()
        {
            _isDisposed = true;
        }
    }
    
    // note, serialize can be expensive and should be called from a background thread
    internal interface IOutgoingQueueItem
    {
        string Address { get; }
        
        byte[] Serialize();
    }
        
    internal struct OutgoingQueueItem<T> : IOutgoingQueueItem
    {
        public string Address { get; set; }
        public T Content;

        public byte[] Serialize() => AddressedEnvelopeUtils.SerializedEnvelope(Address, Content);
    }
    
    internal class RxOutgoingAddressedPackQueue : IOutgoingAddressedPackQueue
    {
        private AddressedPackConnection _connection;
        private bool _isDisposed;
        private Subject<IOutgoingQueueItem> _subject = new ();
        private IDisposable _subscription;

        public RxOutgoingAddressedPackQueue(AddressedPackConnection connection, bool background)
        {
            _connection = connection;

            IObservable<IOutgoingQueueItem> observable = background ? _subject.ObserveOn(Scheduler.ThreadPool) : _subject.AsObservable();

            _subscription = observable.Subscribe(async item =>
                {
                    try
                    {
                        byte[] bytes = item.Serialize();
                        await _connection.Send(bytes);
                        Debug.Log($"WebSocket sent content to address: {item.Address}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("WebSocket send error: " + e.Message);
                        // If you want to re-queue the message:
                        // messageStream.OnNext(message);
                    }
                });
        }

        public void Push<T>(string address, T content)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _subject.OnNext(new OutgoingQueueItem<T> {Address = address, Content = content});
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
            _subject?.Dispose();
            _subject = null;
            _isDisposed = true;
        }
    }
    
    internal class TaskOutgoingAddressedPackQueue : IOutgoingAddressedPackQueue
    {
        private ConcurrentQueue<IOutgoingQueueItem> _queue = new ();
        private AddressedPackConnection _connection;
        private bool _isProcessingQueue;
        private bool _isDisposed;
        
        public TaskOutgoingAddressedPackQueue(AddressedPackConnection connection)
        {
            _connection = connection;
        }
        
        public void Push<T>(string address, T content)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            _queue.Enqueue(new OutgoingQueueItem<T>()
            {
                Address = address,
                Content = content
            });
            
            if (!_isProcessingQueue)
            {
                ProcessQueue().Forget();
            }
        }
        
        private async UniTaskVoid ProcessQueue()
        {
            _isProcessingQueue = true;

            while (!_queue.IsEmpty && !_isDisposed)
            {
                if (_queue.TryDequeue(out IOutgoingQueueItem item))
                {
                    try
                    {
                        byte[] bytes = item.Serialize();
                        await _connection.Send(bytes);
                        Debug.Log($"WebSocket sent content to address: {item.Address}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("WebSocket send error: " + e.Message);
                        // Optionally: Re-queue the message if needed.
                        // messageQueue.Enqueue(message);
                    }
                }
            }

            _isProcessingQueue = false;
        }

        public void Dispose()
        {
            _connection = null;
            _queue = null;
            _isDisposed = true;
        }
    }
    
#endregion

#region Incoming Queue

    internal interface IIncomingAddressedPackQueue : IDisposable
    {
        void Push(byte[] bytes);

        IObservable<T> IncomingContentObservable<T>(string address);
    }

    internal class RxIncomingAddressedPackQueue : IIncomingAddressedPackQueue
    {
        private readonly Subject<byte[]> _incomingSubject = new();
        private readonly Subject<AddressedEnvelope> _envelopeSubject = new();
        private IDisposable _subscription;
        private bool _isDisposed;
        
        public RxIncomingAddressedPackQueue(bool background)
        {
            IObservable<byte[]> observable = background ? 
                _incomingSubject.ObserveOn(Scheduler.ThreadPool) 
                : _incomingSubject.AsObservable();

            void ProcessEnvelope(AddressedEnvelope envelope)
            {
                if (envelope != null)
                {
                    if (envelope.Address == EnvelopeBundle.ADDRESS)
                    {
                        EnvelopeBundle bundle = MessagePackSerializer.Deserialize<EnvelopeBundle>(envelope.Content);
                        Debug.LogWarning($"{nameof(AddressedPackConnection)} deserialized bundle with {bundle.Envelopes.Length} envelopes successfully");
                        foreach (AddressedEnvelope child in bundle.Envelopes)
                        {
                            ProcessEnvelope(child);           
                        }
                    }
                    else
                    {
                        //Debug.LogWarning($"{nameof(AddressedPackConnection)} deserialized envelope successfully");
                        _envelopeSubject.OnNext(envelope);
                        //Debug.LogWarning($"{nameof(AddressedPackConnection)} onNext called successfully");    
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(AddressedPackConnection)} failed to deserialize envelope");
                }
            }
            
            _subscription = observable.Subscribe(bytes =>
            {
                try
                {
                    AddressedEnvelope envelope = MessagePackSerializer.Deserialize<AddressedEnvelope>(bytes);
                    ProcessEnvelope(envelope);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(AddressedPackConnection)} failed to deserialize envelope : ${e.Message}");
                }
            });
        }
        
        public void Push(byte[] bytes)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            _incomingSubject.OnNext(bytes);
        }

        public IObservable<T> IncomingContentObservable<T>(string address)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            return _envelopeSubject.Where(e => e.Address == address)
                .Select(envelope =>
                {
                    try
                    {
                        T content = MessagePackSerializer.Deserialize<T>(envelope.Content);
                        if (content != null)
                        {
                            Debug.Log($"{nameof(RxIncomingAddressedPackQueue)} deserialized content {content}");
                            return Option<T>.Some(content);
                        }
                        Debug.LogError($"{nameof(RxIncomingAddressedPackQueue)} failed to deserialize content");
                        return Option<T>.None;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return Option<T>.None;
                    } 
                }).Where(option => option.HasValue).Select(option => option.Value);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _incomingSubject?.Dispose();
            _envelopeSubject?.Dispose();
            _subscription?.Dispose();
            _subscription = null;
        }
    }

#endregion
    
    
    // note could separate out the websocket to potentially swap it out for any other stream 

    public class AddressedPackConnection : WebSocketClient
    {
        private enum OutgoingQueueType
        {
            Forget,
            Task,
            RxMainThread,
            RxThreadPool
        }

        [SerializeField] private OutgoingQueueType _outgoingQueueType;
        
        private IOutgoingAddressedPackQueue _outgoingQueue;
        private IOutgoingAddressedPackQueue OutgoingQueue => _outgoingQueue ??= CreateOutgoingQueue();

        private enum IncomingQueueType
        {
            RxMainThread,
            RxThreadPool
        }
        
        [SerializeField] private IncomingQueueType _incomingQueueType;

        private IIncomingAddressedPackQueue _incomingQueue;
        private IIncomingAddressedPackQueue IncomingQueue => _incomingQueue ??= CreateIncomingQueue();

        protected virtual void OnValidate()
        {
            _outgoingQueue?.Dispose();
            _outgoingQueue = null;
            
            _incomingQueue?.Dispose();
            _incomingQueue = null;
        }

        private IIncomingAddressedPackQueue CreateIncomingQueue() => _incomingQueueType switch
        {
            IncomingQueueType.RxMainThread => new RxIncomingAddressedPackQueue(false),
            IncomingQueueType.RxThreadPool => new RxIncomingAddressedPackQueue(true),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        protected override void OnMessage(byte[] bytes)
        {
            base.OnMessage(bytes);
            if (!PlayerLoopHelper.IsMainThread)
            {
                Debug.LogWarning($"Unexpected {nameof(OnMessage)} call off main thread ({bytes.Length} bytes)");
            }
            else
            {
                Debug.Log($"{nameof(OnMessage)} call on main thread ({bytes.Length} bytes)");
            }
            IncomingQueue.Push(bytes);
        }

        private IOutgoingAddressedPackQueue CreateOutgoingQueue() => _outgoingQueueType switch
        {
            OutgoingQueueType.Forget => new ForgetOutgoingAddressedPackQueue(this),
            OutgoingQueueType.Task => new TaskOutgoingAddressedPackQueue(this),
            OutgoingQueueType.RxMainThread => new RxOutgoingAddressedPackQueue(this, false),
            OutgoingQueueType.RxThreadPool => new RxOutgoingAddressedPackQueue(this, true),
            _ => throw new ArgumentOutOfRangeException()
        };

        public void QueueOutgoingContent<T>(string address, T content)
        {
            OutgoingQueue.Push(address, content);
        }
        
        public IObservable<T> IncomingContentObservable<T>(string address)
        {
            return IncomingQueue.IncomingContentObservable<T>(address);
        }
    }
}