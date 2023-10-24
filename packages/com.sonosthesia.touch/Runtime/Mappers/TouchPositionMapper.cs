using Sonosthesia.Mapping;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchPositionMapper : Mapper<TouchPayload, Vector3>
    {
        [SerializeField] private Vector3 _offset;

        [SerializeField] private float _scale = 1f;
        
        public override Vector3 Map(TouchPayload source, TouchPayload reference, float timeOffset)
        {
            return _offset + (Vector3)((source.Source.pos - reference.Source.pos) * _scale);
        }
    }
}