using Sonosthesia.Flow;
using Unity.Mathematics;

namespace Sonosthesia.Touch
{
    public readonly struct TouchPayload
    {
        public readonly float3 Contact;
        public readonly float3 Position;
        public readonly float2 UV;
        public readonly float4 Color;

        public TouchPayload(float3 contact, float3 position)
        {
            Contact = contact;
            Position = position;
            UV = float2.zero;
            Color = float4.zero;
        }
        
        public TouchPayload(float3 contact, float3 position, float2 uv, float4 color)
        {
            Contact = contact;
            Position = position;
            UV = uv;
            Color = color;
        }

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Contact)} {Contact} {nameof(Position)} {Position} {nameof(UV)} {UV} {nameof(Color)} {Color}";
        }
    }
    
    public class TouchChannel : Channel<TouchPayload>
    {
        
    }
}

