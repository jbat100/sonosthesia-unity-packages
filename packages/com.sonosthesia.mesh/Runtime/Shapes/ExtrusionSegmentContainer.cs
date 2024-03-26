using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [CreateAssetMenu(fileName = "ExtrusionSegmentContainer", menuName = "Sonosthesia/Mesh/ExtrusionSegmentContainer")]
    public class ExtrusionSegmentContainer : ScriptableObject
    {
        [SerializeField] private List<ExtrusionSegmentData> _segments;

        private NativeArray<ExtrusionSegment> _nativeSegments;

        public int SegmentCount => _segments.Count;
        
        public NativeArray<ExtrusionSegment> NativeSegments
        {
            get
            {
                if (_nativeSegments.IsCreated)
                {
                    return _nativeSegments;
                }

                _nativeSegments = new NativeArray<ExtrusionSegment>(_segments.Select(p => p.Segment()).ToArray(), Allocator.Persistent);
                return _nativeSegments;
            }
        }

        protected virtual void OnValidate()
        {
            _nativeSegments.Dispose();
            _nativeSegments = default;
        }

        protected virtual void OnDisable()
        {
            _nativeSegments.Dispose();
            _nativeSegments = default;
        }
    }
}