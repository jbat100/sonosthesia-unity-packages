using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

namespace Sonosthesia.Channel
{
    public class ChannelInstantiator<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Channel<T> _source;
        
        [SerializeField] private GameObject _prefab;

        [SerializeField] private Transform _attach;

        [SerializeField] private bool _poolCollectionChecks = true;
        
        [SerializeField] private int _maxPoolSize = 10;
        
        private ObjectPool<GameObject> _pool;

        protected void Awake()
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

        private IDisposable _subscription;
        private readonly Dictionary<GameObject, IStreamHandler<T>[]> _alive = new();

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(Instantiate);
            }
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected virtual void Instantiate(IObservable<T> stream)
        {
            GameObject spawn = _pool.Get();
            if (!_alive.TryGetValue(spawn, out IStreamHandler<T>[] handlers))
            {
                handlers = spawn.GetComponentsInChildren<IStreamHandler<T>>(false);
                _alive[spawn] = handlers;
            }
            IEnumerable<IObservable<Unit>> completions = handlers.Select(handler => handler.HandleStream(stream));
            // merge completes when all handlers have completed
            completions.Merge().Subscribe(
                _ =>
                {
                    
                },
                error =>
                {
                    Debug.LogError($"{this} release on handler error {error.Message}");
                    _pool.Release(spawn);
                },
                () =>
                {
                    Debug.Log($"{this} release on handler completion");
                    _pool.Release(spawn);
                });
        }
    }

}