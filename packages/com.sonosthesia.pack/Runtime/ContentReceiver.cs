using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class ContentReceiver<T> : MonoBehaviour
    {
        [SerializeField]
        private PackReceiver _receiver;

        private IDisposable _subscription;
        private readonly Subject<T> _subject = new ();

        public IObservable<T> ContentObservable => _subject.AsObservable();

        protected virtual void Awake()
        {
            if (!_receiver)
            {
                _receiver = GetComponentInParent<PackReceiver>();
            }
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _receiver.PublishContent<T>().Subscribe(_subject);
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}