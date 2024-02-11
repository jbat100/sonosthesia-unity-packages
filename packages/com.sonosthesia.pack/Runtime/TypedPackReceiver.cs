using System;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using UniRx;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class TypedEnvelope
    {
        [Key("type")]
        public int Type { get; set; }

        [Key("content")]
        public byte[] Content { get; set; }
    }

    public class TypedPackReceiver : MonoBehaviour
    {
        [SerializeField] private Wire _wire;
        
        private readonly Subject<TypedEnvelope> _envelopeSubject = new();

        private readonly Dictionary<Type, int> _types = new Dictionary<Type, int>
        {
            {typeof(Counting), 1},
            {typeof(Point), 2},
            {typeof(MediapipePose), 1000}
        };

        private IDisposable _messageSubscription;

        protected virtual void OnEnable()
        {
            _messageSubscription?.Dispose();
            _messageSubscription = _wire.MessageObservable.Subscribe(bytes =>
            {
                TypedEnvelope envelope = MessagePackSerializer.Deserialize<TypedEnvelope>(bytes);
                Debug.Log($"Received {nameof(TypedEnvelope)} with type {envelope.Type}");
                _envelopeSubject.OnNext(envelope);
            });
        }

        protected virtual void OnDisable() => _messageSubscription?.Dispose();
        
        public IObservable<T> PublishContent<T>()
        {
            if (!_types.TryGetValue(typeof(T), out int type))
            {
                throw new Exception("unsupported type");
            }

            return _envelopeSubject.Where(e => e.Type == type)
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
                .AsObservable();
        }
    }
}