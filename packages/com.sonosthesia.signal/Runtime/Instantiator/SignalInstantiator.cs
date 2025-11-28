using System;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using Sonosthesia.Utils;

namespace Sonosthesia.Signal
{
    public class SignalInstantiator<T> : MonoBehaviour where T: struct
    {
        [SerializeField] private InterfaceReference<ISignal<T>> _source;
        
        [SerializeField] private GameObject _prefab;

        [SerializeField] private Transform _attach;

        [SerializeField] private float _lifetime = 1;

        [SerializeField] private Selector<T> _scaleSelector;
        
        [SerializeField] private bool _poolCollectionChecks = true;
        
        [SerializeField] private int _maxPoolSize = 10;
        
        private ObjectPool<GameObject> _pool;
        private IDisposable _subscription;
        
        protected virtual void Awake()
        {
            _pool = new ObjectPool<GameObject>(() => Instantiate(_prefab), 
                o =>
                {
                    o.transform.SetParent(_attach ? _attach : transform, false);
                    o.SetActive(true);
                }, 
                o => o.SetActive(false), 
                Destroy, 
                _poolCollectionChecks, 
                10, 
                _maxPoolSize);
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.Value?.Observable.Subscribe(Instantiate);
        }

        protected virtual void Instantiate(T value)
        {
            GameObject spawn = _pool.Get();
            
            if (_scaleSelector)
            {
                float scale = _scaleSelector.Select(value);
                spawn.transform.localScale = Vector3.one * scale;
            }

            Observable.Timer(TimeSpan.FromSeconds(_lifetime)).Subscribe(_ => _pool.Release(spawn));
        }
    }
}