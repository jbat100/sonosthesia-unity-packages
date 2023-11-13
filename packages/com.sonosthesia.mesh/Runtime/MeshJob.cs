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
            => ScheduleParallel(mesh, meshData, resolution, dependency, Vector3.zero, false);
        
        public static JobHandle ScheduleParallel(
            UnityEngine.Mesh mesh, UnityEngine.Mesh.MeshData meshData, int resolution, JobHandle dependency,
            Vector3 extraBoundsExtents, bool supportVectorization) 
        {
            MeshJob<G, S> job = new MeshJob<G, S>();
            job._generator.Resolution = resolution;
            
            int vertexCount = job._generator.VertexCount;
            
            if (supportVectorization && (vertexCount & 0b11) != 0) 
            {
                vertexCount += 4 - (vertexCount & 0b11);
            }
            
            Bounds bounds = job._generator.Bounds;
            bounds.extents += extraBoundsExtents;
            
            job._streams.Setup(
                meshData, 
                mesh.bounds = bounds, 
                vertexCount, 
                job._generator.IndexCount);
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