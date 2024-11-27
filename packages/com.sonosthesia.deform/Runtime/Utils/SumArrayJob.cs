using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct SumArrayJob<T> : IJobFor where T : struct, ISummable<T>
    {
        [ReadOnly] public NativeArray<T> source;
        public NativeArray<T> target;

        public void Execute(int index)
        {
            target[index] = source[index].Sum(target[index]);
        }
    }
    
    // code duplication as we don't have numeric type constraints for templates
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct FloatSumArrayJob : IJobFor
    {
        [ReadOnly] public NativeArray<float> source;
        public NativeArray<float> target;

        public void Execute(int index)
        {
            target[index] = source[index] + target[index];
        }
    }
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct Float4SumArrayJob : IJobFor
    {
        [ReadOnly] public NativeArray<float4> source;
        public NativeArray<float4> target;

        public void Execute(int index)
        {
            target[index] = source[index] + target[index];
        }
    }
}