using System.Collections.Generic;
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
        
        public void Populate(ref NativeArray<ExtrusionSegment> segments)
        {
            segments.TryReusePersistentArray(SegmentCount);
            for (int i = 0; i < segments.Length; ++i)
            {
                segments[i] = _segments[i].Segment();
            }
        }
    }
}