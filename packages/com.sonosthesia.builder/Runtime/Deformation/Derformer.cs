using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Builder
{
    public interface IDeformer
    {
        Sample4 GetDeformation4(float4x3 positions, float time);
    }

    public static class Deformer
    {
        public delegate JobHandle ScheduleDelegate (
            NativeArray<float3x4> positions, NativeArray<float4> noise,
            SpaceTRS domainTRS, int resolution, float time, JobHandle dependency
        );        
    }
    

}