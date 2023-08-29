using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public delegate JobHandle SurfaceJobScheduleDelegate (
        Mesh.MeshData meshData, int resolution, Noise.Settings settings, int seed, SpaceTRS domain,
        float displacement, bool isPlane, JobHandle dependency
    );
    
    public struct Vertex4 
    {
        public SingleStreams.Stream0 v0, v1, v2, v3;
    }
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct SurfaceJob<N> : IJobFor where N : struct, Noise.INoise
    {
        private Noise.Settings settings;
        private int seed;
        private float3x4 domainTRS;
        private float3x3 derivativeMatrix;
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            Sample4 noise = Noise.GetFractalNoise<N>(v, domainTRS, settings, seed) * displacement;
            noise.Derivatives = derivativeMatrix.TransformVectors(noise.Derivatives);
            vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
        }
        
        public static JobHandle ScheduleParallel (Mesh.MeshData meshData, int resolution, 
            Noise.Settings settings, int seed, SpaceTRS domain, float displacement, bool isPlane, 
            JobHandle dependency
            ) => 
            new SurfaceJob<N>
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                settings = settings,
                seed = seed,
                domainTRS = domain.Matrix,
                derivativeMatrix = domain.DerivativeMatrix,
                displacement = displacement,
                isPlane = isPlane
            }.ScheduleParallel(meshData.vertexCount / 4, resolution, dependency);
    }    
}
