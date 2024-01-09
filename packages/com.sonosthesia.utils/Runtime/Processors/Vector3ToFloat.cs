using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class Vector3ToFloat
    {
        [SerializeField] private Axes _filter;

        [SerializeField] private Vector3Selector _selector;

        public float ExtractFloat(Vector3 v)
        {
            Vector3 filtered = v.FilterAxes(_filter);
            return filtered.Select(_selector);
        }
        
    }
}