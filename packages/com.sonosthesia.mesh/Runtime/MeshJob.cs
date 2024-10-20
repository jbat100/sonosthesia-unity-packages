using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct MeshJob<G, S> : IJobFor
        where G : struct, IMeshGenerator
        where S : struct, IMeshStreams
    {
        private G _generator;
        
        [WriteOnly] private S _streams;

        public void Execute (int i) => _generator.Execute(i, _streams);

        public static JobHandle ScheduleParallel(UnityEngine.Mesh mesh, UnityEngine.Mesh.MeshData meshData, int resolution, JobHandle dependency)
            => ScheduleParallel(mesh, meshData, resolution, dependency, Vector3.zero, true);

        public static void GetInfo(int resolution, bool supportVectorization, out int vertexCount, out int indexCount, out Bounds bounds)
        {
            MeshJob<G, S> job = new MeshJob<G, S>();
            job._generator.Resolution = resolution;
            vertexCount = job._generator.VertexCount;
            // ensure vertex count is a multiple of 4
            if (supportVectorization && (vertexCount & 0b11) != 0) 
            {
                vertexCount += 4 - (vertexCount & 0b11);
            }
            bounds = job._generator.Bounds;
            indexCount = job._generator.IndexCount;
        }
        
        public static JobHandle ScheduleParallel(
            UnityEngine.Mesh mesh, UnityEngine.Mesh.MeshData meshData, int resolution, JobHandle dependency,
            Vector3 extraBoundsExtents, bool supportVectorization)
        {
            GetInfo(resolution, supportVectorization, out int vertexCount, out int indexCount, out Bounds bounds);
            
            MeshJob<G, S> job = new MeshJob<G, S>();
            job._generator.Resolution = resolution;
            bounds.extents += extraBoundsExtents;
            
            job._streams.Setup(
                meshData, 
                mesh.bounds = bounds, 
                vertexCount, 
                indexCount);
            return job.ScheduleParallel(job._generator.JobLength, 1, dependency);
        }
    }
    
    public delegate JobHandle MeshJobScheduleDelegate (
        UnityEngine.Mesh mesh, UnityEngine.Mesh.MeshData meshData, int resolution, JobHandle dependency
    );
    
    public delegate JobHandle AdvancedMeshJobScheduleDelegate (
        UnityEngine.Mesh mesh, UnityEngine.Mesh.MeshData meshData, int resolution, JobHandle dependency,
        Vector3 extraBoundsExtents, bool supportVectorization
    );
}