using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public readonly struct TouchPayload
    {
        public readonly float3 Contact;
        public readonly float3 Normal;
        public readonly float2 UV;
        public readonly float4 Color;
        public readonly RigidTransform Transform;
        public readonly RigidTransform Velocity;
        public readonly float Seperation;

        public TouchPayload(ContactPoint contactPoint, RigidTransform transform, RigidTransform velocity)
        {
            Contact = contactPoint.point;
            Normal = contactPoint.normal;
            Seperation = contactPoint.separation;
            Transform = default;
            Velocity = default;
            Normal = default;
            UV = default;
            Color = default;
        }

        public TouchPayload(float3 contact, RigidTransform transform, RigidTransform velocity)
        {
            Contact = contact;
            Transform = transform;
            Velocity = velocity;
            Normal = float3.zero;
            UV = default;
            Color = default;
            Seperation = 0f;
        }
        
        public TouchPayload(float3 contact, RigidTransform transform, RigidTransform velocity, float2 uv, float4 color)
        {
            Contact = contact;
            Transform = transform;
            Velocity = velocity;
            Normal = float3.zero;
            UV = uv;
            Color = color;
            Seperation = 0f;
        }

        public TouchPayload Updated(RigidTransform transform, RigidTransform velocity)
        {
            return new TouchPayload(Contact, transform, velocity, UV, Color);
        }

        public float3 Position => Transform.pos;
        
        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Contact)} {Contact} {nameof(Transform)} {Transform} {nameof(Velocity)} {Velocity} {nameof(UV)} {UV} {nameof(Color)} {Color}";
        }
    }
    
    public class TouchChannel : Channel<TouchPayload>
    {
        
    }
}

