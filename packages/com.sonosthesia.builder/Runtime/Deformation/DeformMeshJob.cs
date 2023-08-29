using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct DeformMeshJob : IJobFor
    {
        private float3x3 derivativeMatrix;
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;
        private NativeArray<Sample4> defomations;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            Sample4 noise = defomations[i]  * displacement;
            noise.Derivatives = derivativeMatrix.TransformVectors(noise.Derivatives);
            vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
        }

        public static JobHandle ScheduleParallel (Mesh.MeshData meshData, NativeArray<Sample4> defomations, 
            SpaceTRS domain, float displacement, bool isPlane, int innerloopBatchCount, JobHandle dependency
            ) => new DeformMeshJob
            {
                vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
                defomations = defomations,
                derivativeMatrix = domain.DerivativeMatrix,
                displacement = displacement,
                isPlane = isPlane
            }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
    }    
}