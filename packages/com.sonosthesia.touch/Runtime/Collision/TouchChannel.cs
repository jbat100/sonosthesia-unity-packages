using Sonosthesia.Channel;
using Sonosthesia.Dynamic;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchPayloadBuilder
    {
        public float3 Contact;
        public float3 Normal;
        public float2 UV;
        public float4 Color;
        public RigidTransform Source;
        public RigidTransform Target;
        public TransformDynamics Dynamics;
        public float Seperation;

        public TouchPayloadBuilder()
        {
            
        }

        public TouchPayloadBuilder(TouchPayload payload)
        {
            Contact = payload.Contact;
            Normal = payload.Normal;
            UV = payload.UV;
            Color = payload.Color;
            Source = payload.Source;
            Target = payload.Target;
            Dynamics = payload.Dynamics;
            Seperation = payload.Separation;
        }
        
        public void Apply(ContactPoint contactPoint)
        {
            Contact = contactPoint.point;
            Normal = contactPoint.normal;
            Seperation = contactPoint.separation;
        }

        public TouchPayload ToTouchPayload()
        {
            return new TouchPayload(Contact, Normal, UV, Color, Source, Target, Dynamics, Seperation);
        }
    }
    
    public readonly struct TouchPayload
    {
        public readonly float3 Contact;
        public readonly float3 Normal;
        public readonly float2 UV;
        public readonly float4 Color;
        public readonly RigidTransform Source;
        public readonly RigidTransform Target;
        public readonly TransformDynamics Dynamics;
        public readonly float Separation;

        public TouchPayload(
            float3 contact, 
            float3 normal, 
            float2 uv, 
            float4 color, 
            RigidTransform source, 
            RigidTransform target,
            TransformDynamics dynamics,
            float separation)
        {
            Contact = contact;
            Normal = normal;
            UV = uv;
            Source = source;
            Target = target;
            Dynamics = dynamics;
            Color = color;
            Separation = separation;
        }
        
        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Contact)} {Contact} {nameof(Dynamics)} {Dynamics} {nameof(UV)} {UV} {nameof(Color)} {Color}";
        }
    }
    
    public class TouchChannel : Channel<TouchPayload>
    {
        
    }
}

