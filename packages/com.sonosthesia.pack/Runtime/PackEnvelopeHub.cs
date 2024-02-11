using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly Wire _wire;
        
        public ForgetOutgoingAddressedPackQueue(Wire wire)
        {
            _wire = wire;
        }
        
        public void Push<T>(string address, T content)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _wire.Send(AddressedEnvelopeUtils.SerializedEnvelope(address, content)).Forget();
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
        private Wire _wire;
        private bool _isDisposed;
        private Subject<IOutgoingQueueItem> _subject = new ();
        private IDisposable _subscription;

        public RxOutgoingAddressedPackQueue(Wire wire, bool background)
        {
            _wire = wire;

            IObservable<IOutgoingQueueItem> observable = background ? _subject.ObserveOn(Scheduler.ThreadPool) : _subject.AsObservable();

            // TODO : switch to R3 to handle async subscribe
            
            _subscription = observable.Subscribe(async item =>
                {
                    try
                    {
                        byte[] bytes = item.Serialize();
                        await _wire.Send(bytes);
                        //Debug.Log($"WebSocket sent content to address: {item.Address}");
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
            _wire = null;
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
        private Wire _wire;
        private bool _isProcessingQueue;
        private bool _isDisposed;
        
        public TaskOutgoingAddressedPackQueue(Wire wire)
        {
            _wire = wire;
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
                        await _wire.Send(bytes);
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
            _wire = null;
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
        // entry point for byte arrays, background constructor arg determines if observed on main thread or thread pool
        private readonly Subject<byte[]> _incomingSubject = new();
        
        // TODO : try using pools but it is not easy with background PooledObject, could make things easier
        // envelope subjects by address, if address is not request, AddressedEnvelope is ignored
        private readonly Dictionary<string, Subject<AddressedEnvelope>> _envelopeSubjects = new();
        
        private IDisposable _subscription;
        private bool _isDisposed;
        private bool _log;
        
        public RxIncomingAddressedPackQueue(bool background, bool log)
        {
            _log = log;
            
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
                        if (_log)
                        {
                            Debug.Log($"{nameof(PackEnvelopeHub)} deserialized bundle with {bundle.Envelopes.Length} envelopes successfully");
                        }
                        foreach (AddressedEnvelope child in bundle.Envelopes)
                        {
                            ProcessEnvelope(child);           
                        }
                    }
                    else
                    {
                        if (_envelopeSubjects.TryGetValue(envelope.Address, out Subject<AddressedEnvelope> subject))
                        {
                            //Debug.LogWarning($"{nameof(AddressedPackConnection)} deserialized envelope");
                            subject.OnNext(envelope);
                            //Debug.LogWarning($"{nameof(AddressedPackConnection)} onNext called successfully");                              
                        }
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(PackEnvelopeHub)} failed to deserialize envelope");
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
                    Debug.LogError($"{nameof(PackEnvelopeHub)} failed to deserialize envelope : ${e.Message}");
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

        private readonly Dictionary<string, object> _contentObservables = new();
        
        private IObservable<T> MakeIncomingContentObservable<T>(string address)
        {
            Subject<AddressedEnvelope> envelopeSubject = _envelopeSubjects.Ensure(address);

            return envelopeSubject.Where(e => e.Address == address)
                .Select(envelope =>
                {
                    try
                    {
                        T content = MessagePackSerializer.Deserialize<T>(envelope.Content);
                        if (content != null)
                        {
                            if (_log)
                            {
                                Debug.Log($"{nameof(RxIncomingAddressedPackQueue)} deserialized content {content}");
                            }

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
        
        public IObservable<T> IncomingContentObservable<T>(string address)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            object result = _contentObservables.Ensure(address, () => MakeIncomingContentObservable<T>(address));

            if (result is IObservable<T> observable)
            {
                return observable.AsObservable();
            }

            throw new Exception($"Mismatched requests for address {address}");
        }

        public void Dispose()
        {
            _isDisposed = true;
            _incomingSubject?.Dispose();
            _subscription?.Dispose();
            _subscription = null;

            foreach (Subject<AddressedEnvelope> envelopeSubject in _envelopeSubjects.Values)
            {
                envelopeSubject.OnCompleted();
                envelopeSubject.Dispose();
            }

            _envelopeSubjects.Clear();
            _contentObservables.Clear();
        }
    }

#endregion
    
    
    // note could separate out the websocket to potentially swap it out for any other stream 

    public class PackEnvelopeHub : MonoBehaviour
    {
        [SerializeField] private bool _log;

        [SerializeField] private Wire _wire;

        private IDisposable _messageSubscription;
        
        private enum OutgoingQueueType
        {
            Forget,
            Task,
            RxMainThread,
            RxThreadPool
        }

        [SerializeField] private OutgoingQueueType _outgoingQueueType = OutgoingQueueType.RxThreadPool;
        
        private IOutgoingAddressedPackQueue _outgoingQueue;
        private IOutgoingAddressedPackQueue OutgoingQueue => _outgoingQueue ??= CreateOutgoingQueue();

        private enum IncomingQueueType
        {
            RxMainThread,
            RxThreadPool
        }
        
        [SerializeField] private IncomingQueueType _incomingQueueType = IncomingQueueType.RxThreadPool;

        private IIncomingAddressedPackQueue _incomingQueue;
        private IIncomingAddressedPackQueue IncomingQueue => _incomingQueue ??= CreateIncomingQueue();

        protected virtual void Awake()
        {
            _messageSubscription = _wire.MessageObservable
                .ObserveOnMainThread()
                .Subscribe(bytes => IncomingQueue.Push(bytes));
        }
        
        protected virtual void OnValidate()
        {
            _messageSubscription?.Dispose();
            
            _outgoingQueue?.Dispose();
            _outgoingQueue = null;
            
            _incomingQueue?.Dispose();
            _incomingQueue = null;
            
            _messageSubscription = _wire.MessageObservable.Subscribe(bytes => IncomingQueue.Push(bytes));
        }

        private IIncomingAddressedPackQueue CreateIncomingQueue() => _incomingQueueType switch
        {
            IncomingQueueType.RxMainThread => new RxIncomingAddressedPackQueue(false, _log),
            IncomingQueueType.RxThreadPool => new RxIncomingAddressedPackQueue(true, _log),
            _ => throw new ArgumentOutOfRangeException()
        };

        private IOutgoingAddressedPackQueue CreateOutgoingQueue() => _outgoingQueueType switch
        {
            OutgoingQueueType.Forget => new ForgetOutgoingAddressedPackQueue(_wire),
            OutgoingQueueType.Task => new TaskOutgoingAddressedPackQueue(_wire),
            OutgoingQueueType.RxMainThread => new RxOutgoingAddressedPackQueue(_wire, false),
            OutgoingQueueType.RxThreadPool => new RxOutgoingAddressedPackQueue(_wire, true),
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