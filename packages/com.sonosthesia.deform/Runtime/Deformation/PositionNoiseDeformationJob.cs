using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct PositionNoiseDeformationJob<N> : IJobFor where N : struct, Noise.INoise
    {
        [ReadOnly] public NativeArray<float3x4> positions;

        [WriteOnly] public NativeArray<Sample4> deformation;

        public Noise.Settings settings;

        public float3x4 domainTRS;

        public int seed;
        
        public float lerp;

        public float displacement;
        
        public void Execute (int i)
        {
            Sample4 d1 = Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(positions[i])), settings, seed
            );

            Sample4 d2 = Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(transpose(positions[i])), settings, seed + 1
            );

            deformation[i] = d1 * (1 - lerp) + d2 * lerp;
        }

        public static JobHandle ScheduleParallel (
            NativeArray<float3x4> positions, NativeArray<Sample4> deformation,
            Noise.Settings settings, int seed, SpaceTRS domainTRS, int innerloopBatchCount, float lerp, JobHandle dependency
        ) => new PositionNoiseDeformationJob<N> {
            positions = positions,
            deformation = deformation,
            settings = settings,
            domainTRS = domainTRS.Matrix,
            seed = seed,
            lerp = lerp
        }.ScheduleParallel(positions.Length, innerloopBatchCount, dependency);

    }
    
    public delegate JobHandle PositionNoiseDeformationJobScheduleDelegate (
        NativeArray<float3x4> positions, NativeArray<Sample4> deformation,
        Noise.Settings settings, int seed, SpaceTRS domainTRS, int innerloopBatchCount, float lerp, JobHandle dependency
    );
}