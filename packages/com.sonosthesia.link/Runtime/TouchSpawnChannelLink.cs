using Sonosthesia.Touch;
using Sonosthesia.Flow;
using Sonosthesia.Spawn;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Link
{
    public class TouchSpawnChannelLink : ChannelLink<TouchPayload, SpawnPayload>
    {
        [Header("Mappers")]
        
        [SerializeField] private Mapper<TouchPayload, Color> _colorMapper;

        [SerializeField] private Mapper<TouchPayload, Vector3> _positionMapper;
        
        [SerializeField] private Mapper<TouchPayload, Quaternion> _rotationMapper;

        [SerializeField] private Mapper<TouchPayload, float> _sizeMapper;
        
        [SerializeField] private Mapper<TouchPayload, float> _lifetimeMapper;
        
        [Header("Providers")]
        
        [SerializeField] private ValueProvider<Color> _colorProvider;

        [SerializeField] private ValueProvider<Vector3> _positionProvider;
        
        [SerializeField] private ValueProvider<Quaternion> _rotationProvider;

        [SerializeField] private ValueProvider<float> _sizeProvider;
        
        [SerializeField] private ValueProvider<float> _lifetimeProvider;
        
        protected override SpawnPayload Map(TouchPayload payload, TouchPayload reference, float timeOffset)
        {
             
            Vector3 position = Map(_positionMapper, _positionProvider, Vector3.zero, payload, reference, timeOffset);
            Quaternion rotation = _rotationMapper ? _rotationMapper.Map(payload, reference, timeOffset) : Quaternion.identity;
            float size = _sizeMapper ? _sizeMapper.Map(payload, reference, timeOffset) : 1f;
            float lifetime = _lifetimeMapper ? _lifetimeMapper.Map(payload, reference, timeOffset) : 1f;
            Color color = _colorMapper ? _colorMapper.Map(payload, reference, timeOffset) : Color.white;
            
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
