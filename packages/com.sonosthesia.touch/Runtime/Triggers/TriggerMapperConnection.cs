using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Mapping;

namespace Sonosthesia.Touch
{
    public class TriggerMapperConnection<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private MapperTarget<TValue> _target;

        [SerializeField] private Mapper<TValue> _mapper;

        private Dictionary<Collider, IDisposable> _connections = new();

        private readonly HashSet<string> _mapperCompatibility = new();
        
        private readonly HashSet<string> _sourceCompatibility = new();

        protected virtual void OnEnable()
        {
            ClearAllConnections();
            _mapperCompatibility.Clear();
            _mapperCompatibility.UnionWith(_mapper.Compatibility);
        }

        protected virtual void OnDisable()
        {
            ClearAllConnections();
        }

        protected virtual void OnValidate()
        {
            _mapperCompatibility.Clear();
            _mapperCompatibility.UnionWith(_mapper.Compatibility);
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            ClearConnection(other);
            
            MapperSource<TValue>[] sources = other.GetComponentsInParent<MapperSource<TValue>>();
            foreach (MapperSource<TValue> source in sources)
            {
                _sourceCompatibility.Clear();
                _sourceCompatibility.UnionWith(source.Compatibility);

                if (_mapperCompatibility.Intersect(_sourceCompatibility).Any())
                {
                    _connections[other] = _mapper.Map(source, _target);
                }
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            ClearConnection(other);
        }

        private void ClearAllConnections()
        {
            foreach (IDisposable subscription in _connections.Values)
            {
                subscription.Dispose();
            }
            _connections.Clear();
        }
        
        private void ClearConnection(Collider other)
        {
            if (_connections.TryGetValue(other, out IDisposable subscription))
            {
                subscription?.Dispose();
                _connections.Remove(other);
            }
        }
    }
}