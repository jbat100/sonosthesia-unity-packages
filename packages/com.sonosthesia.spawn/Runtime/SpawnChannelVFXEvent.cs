using System;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Spawn
{
    public class SpawnChannelVFXEvent : MonoBehaviour
    {
        [SerializeField] private SpawnChannel _source;
        
        [SerializeField] private VisualEffect _visualEffect;
        
        [SerializeField] private string _eventName;
        
        // TODO : use pool https://www.youtube.com/watch?v=uwxFMPUTzW4
        private class SpawnStreamHandler
        {
            private IDisposable _subscription;
            private string _eventName;
            private VisualEffect _visualEffect;

            public void Setup(VisualEffect visualEffect, string eventName, IObservable<SpawnPayload> stream)
            {   
                _subscription?.Dispose();
                _eventName = eventName;
                _visualEffect = visualEffect;
                _subscription = stream.Subscribe(Spawn);
            }

            public void Teardown()
            {
                _subscription?.Dispose();
                _eventName = null;
                _visualEffect = null;
            }

            private void Spawn(SpawnPayload payload)
            {
                VFXEventAttribute eventAttribute = _visualEffect.CreateVFXEventAttribute();
                ConfigureAttribute(eventAttribute, payload);
                _visualEffect.SendEvent(_eventName, eventAttribute);
            }

            private void ConfigureAttribute(VFXEventAttribute eventAttribute, SpawnPayload payload)
            {
                eventAttribute.SetVector3("color", new Vector3(payload.Color.r, payload.Color.g, payload.Color.b));
                eventAttribute.SetVector3("position", payload.Trans.pos);
                eventAttribute.SetFloat("size", payload.Size);
                eventAttribute.SetFloat("lifetime", payload.Lifetime);
            }
        }

        private IDisposable _subscription;

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(stream =>
                {
                    // TODO : check GC / Pooling
                    SpawnStreamHandler handler = new SpawnStreamHandler();
                    handler.Setup(_visualEffect, _eventName, stream);
                });
            }
        }
        
        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}