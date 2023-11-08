using System;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class MapperConnector<TValue> : MonoBehaviour where TValue : struct
    {
        [SerializeField] private MapperConnection<TValue> _source;

        [SerializeField] private MapperConnection<TValue> _target;

        [SerializeField] private Mapper<TValue> _mapper;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_mapper)
            {
                _subscription = _mapper.Map(_source, _target);
            }
            else
            {
                Mapper<TValue>.AutoMap(_source, _target);
            }
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}