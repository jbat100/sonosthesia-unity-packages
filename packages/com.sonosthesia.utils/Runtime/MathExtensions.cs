using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    public static class MathExtensions
    {
        public static float3 Horizontal(this float3 v)
        {
            return new float3(v.x, 0f, v.z);
        }
        
        public static float3 Vertical(this float3 v)
        {
            return new float3(0f, v.y, 0f);
        }
    }
}