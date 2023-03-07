using Sonosthesia.Touch;
using Sonosthesia.Flow;
using Sonosthesia.Spawn;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Link
{
    public class TouchSpawnChannelLink : ChannelLink<TouchPayload, SpawnPayload>
    {
        [Header("Mappings")]
        
        [SerializeField] private Mapping<Color> _colorMapping;
        
        [SerializeField] private Mapping<Vector3> _positionMapping;

        [SerializeField] private Mapping<Quaternion> _rotationMapping;
        
        [SerializeField] private Mapping<float> _sizeMapping;
        
        [SerializeField] private Mapping<float> _lifetimeMapping;

        protected override SpawnPayload Map(TouchPayload payload, TouchPayload reference, float timeOffset)
        {
            Vector3 position = _positionMapping.Map(payload, reference, timeOffset);
            Quaternion rotation = _rotationMapping.Map(payload, reference, timeOffset);
            float size = _sizeMapping.Map(payload, reference, timeOffset);
            float lifetime = _lifetimeMapping.Map(payload, reference, timeOffset);;
            Color color = _colorMapping.Map(payload, reference, timeOffset);;
            
            return new SpawnPayload(
                new RigidTransform(Quaternion.identity, position),
                size,
                lifetime,
                color,
                Vector3.zero, 
                Vector3.zero);
        }
    }
}
