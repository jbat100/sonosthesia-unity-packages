using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using Sonosthesia.Spawn;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Link
{
    public class MIDISpawnChannelLink : ChannelLink<MIDINote, SpawnPayload>
    {
        [SerializeField] private Mapper<MIDINote, Color> _colorMapper;

        [SerializeField] private Mapper<MIDINote, Vector3> _positionMapper;

        [SerializeField] private Mapper<MIDINote, Quaternion> _rotationMapper;
        
        [SerializeField] private Mapper<MIDINote, float> _sizeMapper;
        
        [SerializeField] private Mapper<MIDINote, float> _lifetimeMapper;
        
        protected override SpawnPayload Map(MIDINote payload, MIDINote reference, float timeOffset)
        {
            Vector3 position = _positionMapper ? _positionMapper.Map(payload, reference, timeOffset) : Vector3.zero;
            Quaternion rotation = _rotationMapper ? _rotationMapper.Map(payload, reference, timeOffset) : Quaternion.identity;
            float size = _sizeMapper ? _sizeMapper.Map(payload, reference, timeOffset) : 1f;
            float lifetime = _lifetimeMapper ? _lifetimeMapper.Map(payload, reference, timeOffset) : 1f;
            Color color = _colorMapper ? _colorMapper.Map(payload, reference, timeOffset) : Color.white;
            
            return new SpawnPayload(
                new RigidTransform(rotation, position),
                size,
                lifetime,
                color,
                Vector3.zero, 
                Vector3.zero);
        }
    }
}