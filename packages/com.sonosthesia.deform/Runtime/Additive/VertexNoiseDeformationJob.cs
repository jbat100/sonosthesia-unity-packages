using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

using Sonosthesia.Mesh;
using Sonosthesia.Noise;

namespace Sonosthesia.Deform
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    struct VertexNoiseDeformationJob<N> : IJobFor where N : struct, INoise
    {
        [ReadOnly] public NativeArray<Vertex4> vertices;

        [WriteOnly] public NativeArray<Sample4> deformation;

        public FractalSettings settings;

        public float3x4 domainTRS;

        public int seed;
        
        public float lerp;

        public void Execute (int i)
        {
            Vertex4 v = vertices[i];
            float4x3 position = domainTRS.TransformVectors(transpose(float3x4(
                v.v0.position, v.v1.position, v.v2.position, v.v3.position
            )));
            Sample4 d0 = Noise.Noise.GetFractalNoise<N>(position, settings, seed);
            Sample4 d1 = Noise.Noise.GetFractalNoise<N>(position, settings, seed + 1);
            deformation[i] = d0 * (1 - lerp) + d1 * lerp;
        }

        public static JobHandle ScheduleParallel (
            UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformation,
            FractalSettings settings, int seed, SpaceTRS domainTRS, int innerloopBatchCount, float lerp, JobHandle dependency
        ) => new VertexNoiseDeformationJob<N> {
            vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
            deformation = deformation,
            settings = settings,
            domainTRS = domainTRS.Matrix,
            seed = seed,
            lerp = lerp
        }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);

    }
    
    public delegate JobHandle VertexNoiseDeformationJobScheduleDelegate (
        UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformation,
        FractalSettings settings, int seed, SpaceTRS domainTRS, int innerloopBatchCount, float lerp, JobHandle dependency
    );
}