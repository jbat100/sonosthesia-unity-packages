using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using MessagePack;
using NativeWebSocket;

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
    
    public class AddressedPackConnection : WebSocketClient
    {
        [SerializeField] private string _address;
        
        private WebSocket websocket;

        private readonly Subject<AddressedEnvelope> _envelopeSubject = new();
        
        protected override void OnMessage(byte[] bytes)
        {
            base.OnMessage(bytes);
            AddressedEnvelope envelope = MessagePackSerializer.Deserialize<AddressedEnvelope>(bytes);
            Debug.Log($"Received {nameof(AddressedEnvelope)} with address {envelope.Address}");
            _envelopeSubject.OnNext(envelope);
        }
        
        public IObservable<T> PublishContent<T>(string address)
        {
            return _envelopeSubject.Where(e => e.Address == address)
                .ObserveOn(Scheduler.ThreadPool)
                .Select(envelope =>
                {
                    try
                    {
                        return MessagePackSerializer.Deserialize<T>(envelope.Content);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        throw;
                    }
                })
                .ObserveOnMainThread()
                .AsObservable();
        }

        public async UniTask<bool> SendContent<T>(string address, T content)
        {
            await UniTask.SwitchToThreadPool();
            byte[] bytes = MessagePackSerializer.Serialize(content);
            AddressedEnvelope envelope = new AddressedEnvelope()
            {
                Address = address,
                Content = bytes
            };
            return await Send(MessagePackSerializer.Serialize(envelope));
        }
    }
}