using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [CreateAssetMenu(fileName = "ExtrusionShape", menuName = "Sonosthesia/Mesh/ExtrusionShape")]
    public class ExtrusionShape : ScriptableObject
    {
        [Serializable]
        private class PointData
        {
            [SerializeField] private Vector2 _position;
            [SerializeField] private Vector2 _normal;
            [SerializeField] private float _u;

            public ExtrusionShapePoint Point()
            {
                return new ExtrusionShapePoint
                {
                    position = _position,
                    normal = _normal,
                    u = _u
                };
            }
        }
        
        [SerializeField] private List<PointData> _points;

        // NOTE : maybe this should be inferred from 0 point distance but would require custom extrusion code 
        [SerializeField] private bool _doubledVertices;
        public bool DoubledVertices => _doubledVertices;
        
        [SerializeField] private bool _closed;
        public bool Closed => _closed;

        private NativeArray<ExtrusionShapePoint> _nativePoints;

        public int PointCount => _points.Count;
        
        public NativeArray<ExtrusionShapePoint> NativePoints
        {
            get
            {
                if (_nativePoints.IsCreated)
                {
                    return _nativePoints;
                }

                _nativePoints = new NativeArray<ExtrusionShapePoint>(_points.Select(p => p.Point()).ToArray(), Allocator.Persistent);
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