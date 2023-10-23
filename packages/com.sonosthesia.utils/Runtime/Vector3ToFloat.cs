using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class Vector3ToFloat
    {
        [SerializeField] private Axes _filter;

        [SerializeField] private Vector3Selector _selector;

        [SerializeField] private bool _abs;
        
        [SerializeField] private float _scale;

        [SerializeField] private float _offset;

        public float Process(Vector3 v)
        {
            Vector3 filtered = v.FilterAxes(_filter);
            float selected = filtered.Select(_selector);
            selected = _abs ? Mathf.Abs(selected) : selected;
            return _offset + selected * _scale;
        }
        
    }
}