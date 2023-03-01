using Sonosthesia.Flow;
using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Spawn
{
    public readonly struct SpawnPayload
    {
        public readonly RigidTransform Trans;
        public readonly float Size;
        public readonly float Lifetime;
        public readonly Color Color;
        public readonly Vector3 Velocity;
        public readonly Vector3 AngularVelocity;

        public SpawnPayload(RigidTransform trans, float size, float lifetime, Color color)
        {
            Trans = trans;
            Size = size;
            Lifetime = lifetime;
            Color = color;
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
        }
        
        public SpawnPayload(RigidTransform trans, float size, float lifetime, Color color, Vector3 velocity, Vector3 angularVelocity)
        {
            Trans = trans;
            Size = size;
            Lifetime = lifetime;
            Color = color;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }

        public override string ToString()
        {
            return $"{nameof(SpawnPayload)} {nameof(Trans)} {Trans} {nameof(Size)} {Size} {nameof(Color)} {Color}";
        }
    }
    
    public class SpawnChannel : Channel<SpawnPayload>
    {
        
    }
}