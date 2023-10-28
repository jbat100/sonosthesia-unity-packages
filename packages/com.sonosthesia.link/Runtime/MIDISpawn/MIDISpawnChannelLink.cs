using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Mapping;
using Sonosthesia.Spawn;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Link
{
    public class MIDISpawnChannelLink : ChannelLink<MIDINote, SpawnPayload>
    {
        [SerializeField] private LinkMapper<MIDINote, Color> colorLinkMapper;

        [SerializeField] private LinkMapper<MIDINote, Vector3> positionLinkMapper;

        [SerializeField] private LinkMapper<MIDINote, Quaternion> rotationLinkMapper;
        
        [SerializeField] private LinkMapper<MIDINote, float> sizeLinkMapper;
        
        [SerializeField] private LinkMapper<MIDINote, float> lifetimeLinkMapper;
        
        protected override SpawnPayload Map(MIDINote payload, MIDINote reference, float timeOffset)
        {
            Vector3 position = positionLinkMapper ? positionLinkMapper.Map(payload, reference, timeOffset) : Vector3.zero;
            Quaternion rotation = rotationLinkMapper ? rotationLinkMapper.Map(payload, reference, timeOffset) : Quaternion.identity;
            float size = sizeLinkMapper ? sizeLinkMapper.Map(payload, reference, timeOffset) : 1f;
            float lifetime = lifetimeLinkMapper ? lifetimeLinkMapper.Map(payload, reference, timeOffset) : 1f;
            Color color = colorLinkMapper ? colorLinkMapper.Map(payload, reference, timeOffset) : Color.white;
            
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