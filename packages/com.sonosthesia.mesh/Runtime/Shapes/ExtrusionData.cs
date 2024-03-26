using System;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    // note : used for inspector only, then transformed to sequential layout structs for the job system
    
    [Serializable]
    public class ExtrusionPointData
    {
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _normal;
        [SerializeField] private float _u;

        public ExtrusionPoint Point()
        {
            return new ExtrusionPoint
            {
                position = _position,
                normal = _normal,
                u = _u
            };
        }
    }

    [Serializable]
    public class ExtrusionSegmentData
    {
        [SerializeField] private ExtrusionPointData _start;
        [SerializeField] private ExtrusionPointData _end;

        public ExtrusionSegment Segment()
        {
            return new ExtrusionSegment
            {
                start = _start.Point(),
                end = _end.Point()
            };
        }
    }
}