using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    // Thinking about using ease, but worried about compatibility with burst given the use of lambda expressions
    // need to check and potentially use a workaround
    
    public enum FalloffType
    {
        Linear
    }
    
    public readonly struct InfluenceZone
    {
        public readonly float3 Center;
        public readonly float Falloff;
        public readonly FalloffType FalloffType;
    }
}