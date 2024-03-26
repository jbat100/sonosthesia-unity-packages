using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [CreateAssetMenu(fileName = "ExtrusionPointContainer", menuName = "Sonosthesia/Mesh/ExtrusionPointContainer")]
    public class ExtrusionPointContainer : ScriptableObject
    {
        [SerializeField] private List<ExtrusionPointData> _points;
        
        [SerializeField] private bool _closed;
        public bool Closed => _closed;

        private NativeArray<ExtrusionPoint> _nativePoints;

        public int PointCount => _points.Count;
        
        public NativeArray<ExtrusionPoint> NativePoints
        {
            get
            {
                if (_nativePoints.IsCreated)
                {
                    return _nativePoints;
                }

                _nativePoints = new NativeArray<ExtrusionPoint>(_points.Select(p => p.Point()).ToArray(), Allocator.Persistent);
                return _nativePoints;
            }
        }

        protected virtual void OnValidate()
        {
            _nativePoints.Dispose();
            _nativePoints = default;
        }

        protected virtual void OnDisable()
        {
            _nativePoints.Dispose();
            _nativePoints = default;
        }
    }
}