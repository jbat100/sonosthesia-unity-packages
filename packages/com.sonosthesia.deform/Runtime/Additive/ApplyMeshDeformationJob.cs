using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct ApplyMeshSampleDeformationJob : IJobFor
    {
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;
        private NativeArray<Sample4> deformations;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            Sample4 noise = deformations[i]  * displacement;
            vertices[i] = SurfaceUtils.SetVertices(v, noise, isPlane);
        }

        public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, NativeArray<Sample4> deformations, 
            float displacement, bool isPlane, int innerloopBatchCount, JobHandle dependency
        ) => new ApplyMeshSampleDeformationJob
        {
            vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
            deformations = deformations,
            displacement = displacement,
            isPlane = isPlane
        }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
    }  
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct ApplyMeshFloatDeformationJob : IJobFor
    {
        private float displacement;
        private bool isPlane;
        private NativeArray<Vertex4> vertices;
        private NativeArray<float4> deformations;

        public void Execute(int i)
        {
            Vertex4 v = vertices[i];
            float4 noise = deformations[i] * displacement;
            vertices[i] = SurfaceUtils.SetDeformation(v, noise, isPlane);
        }

        public static JobHandle ScheduleParallel (UnityEngine.Mesh.MeshData meshData, NativeArray<float4> deformations, 
            float displacement, bool isPlane, int innerloopBatchCount, JobHandle dependency
        ) => new ApplyMeshFloatDeformationJob
        {
            vertices = meshData.GetVertexData<SingleStreams.Stream0>().Reinterpret<Vertex4>(12 * 4),
            deformations = deformations,
            displacement = displacement,
            isPlane = isPlane
        }.ScheduleParallel(meshData.vertexCount / 4, innerloopBatchCount, dependency);
    }  
}