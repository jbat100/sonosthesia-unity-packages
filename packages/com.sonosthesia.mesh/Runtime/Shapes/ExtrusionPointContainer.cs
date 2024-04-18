using System.Collections.Generic;
using Sonosthesia.Utils;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [CreateAssetMenu(fileName = "ExtrusionPointContainer", menuName = "Sonosthesia/Mesh/ExtrusionPointContainer")]
    public class ExtrusionPointContainer : ObservableScriptableObject
    {
        [SerializeField] private List<ExtrusionPointData> _points;
        
        [SerializeField] private bool _closed;
        public bool Closed => _closed;

        public int PointCount => _points.Count;
        
        public void Populate(ref NativeArray<ExtrusionPoint> points)
        {
            points.TryReusePersistentArray(PointCount);
            for (int i = 0; i < points.Length; ++i)
            {
                points[i] = _points[i].Point();
            }
        }
    }
}