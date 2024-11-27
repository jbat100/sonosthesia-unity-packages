using Unity.Mathematics;

namespace Sonosthesia.Noise
{
    public interface INoise4D
    {
        float Compute(float4 input);
    }
    
    public interface ISIMDNoise4D
    {
        float4 Compute(float4x4 input);
    }

    public struct SNoise4D : INoise4D
    {
        public float Compute(float4 input)
        {
            return noise.snoise(input);
        }
    }

    public struct CNoise4D : INoise4D
    {
        public float Compute(float4 input)
        {
            return noise.cnoise(input);
        }
    }

}