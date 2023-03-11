using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Flow;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

namespace Sonosthesia.Spawn
{
    public class ChannelObjectInstantiator<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private Channel<T> _source;
        
        [SerializeField] private GameObject _prefab;

        [SerializeField] private bool _poolCollectionChecks = true;
        
        [SerializeField] private int _maxPoolSize = 10;
        
        private ObjectPool<GameObject> _pool;

        protected void Awake()
        {
            _pool = new ObjectPool<GameObject>(() => Instantiate(_prefab), 
                o => o.SetActive(true), 
                o => o.SetActive(false), 
                Destroy, 
                _poolCollectionChecks, 
                10, 
                _maxPoolSize);
        }

        private IDisposable _subscription;
        private readonly Dictionary<GameObject, IChannelStreamHandler<T>[]> _alive = new();

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (_source)
            {
                _subscription = _source.StreamObservable.Subscribe(Spawn);
            }
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected virtual void Spawn(IObservable<T> stream)
        {
            GameObject spawn = _pool.Get();
            if (!_alive.TryGetValue(spawn, out IChannelStreamHandler<T>[] handlers))
            {
                handlers = spawn.GetComponentsInChildren<IChannelStreamHandler<T>>();
                _alive[spawn] = handlers;
            }
            IEnumerable<IObservable<Unit>> completions = handlers.Select(handler => handler.HandleStream(stream));
            completions.Merge().Subscribe(_ => { }, () => _pool.Release(spawn));
        }
    }

}