using System;
using UnityEngine;
using UniRx;

namespace Sonosthesia.Pack
{
    public class PackMonitor<T> : MonoBehaviour
    {
        [SerializeField]
        private TypedPackReceiver _receiver;

        private IDisposable _subscription;

        protected virtual void Awake()
        {
            if (!_receiver)
            {
                _receiver = GetComponentInParent<TypedPackReceiver>();
            }
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.PublishContent<T>().Subscribe(content =>
            {
                Debug.Log($"{this} received {content}");
            });
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }

    }
}