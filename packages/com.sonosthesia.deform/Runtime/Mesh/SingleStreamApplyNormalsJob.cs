using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sonosthesia.Mesh;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    // indices 
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexTris
    {
        public ushort count;
        public int a, b, c, d;

        public void Push(int triIndex)
        {
            switch (count)
            {
                case 0:
                    a = triIndex;
                    break;
                case 1:
                    b = triIndex;
                    break;
                case 2:
                    c = triIndex;
                    break;
                case 3:
                    d = triIndex;
                    break;
            }

            if (count < 4)
            {
                count++;
            }
        }
    }

    // is used to have a vertex normal averaged over up to 4 connected faces
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct SingleStreamVertexTrisJob : IJob // can't be IJobFor as we have to write to faceNormals arbitrarily
    {
        [ReadOnly] private readonly NativeArray<TriangleUInt16> triangles;
        
        // we both read and write to vertex Tris
        private NativeArray<VertexTris> vertexTris;

        public SingleStreamVertexTrisJob(NativeArray<TriangleUInt16> triangles, NativeArray<VertexTris> vertexTris)
        {
            this.triangles = triangles;
            this.vertexTris = vertexTris;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Push(ushort vertexIndex, int triIndex)
        {
            VertexTris t = vertexTris[vertexIndex];
            t.Push(triIndex);
            vertexTris[vertexIndex] = t;
        }
        
        public void Execute()
        {
            for (int triIndex = 0; triIndex < triangles.Length; triIndex++)
            {
                TriangleUInt16 triangle = triangles[triIndex];
                Push(triangle.a, triIndex);
                Push(triangle.b, triIndex);
                Push(triangle.c, triIndex);
            }
        }
    }
    
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct SingleStreamFaceNormalsJob : IJobFor
    {
        [ReadOnly] private readonly NativeArray<TriangleUInt16> triangles;
        [ReadOnly] private readonly NativeArray<SingleStreams.Stream0> stream0;
        [WriteOnly] private NativeArray<float3> faceNormals;

        public SingleStreamFaceNormalsJob(NativeArray<TriangleUInt16> triangles,
            NativeArray<SingleStreams.Stream0> stream0, NativeArray<float3> faceNormals)
        {
            this.triangles = triangles;
            this.stream0 = stream0;
            this.faceNormals = faceNormals;
        }
        
        public void Execute(int index)
        {
            TriangleUInt16 triangle = triangles[index];

            float3 p0 = stream0[triangle.a].position;
            float3 p1 = stream0[triangle.b].position;
            float3 p2 = stream0[triangle.c].position;
            
            faceNormals[index] = math.normalize(math.cross(p1-p0, p2-p0));
        }
    }
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct SingleStreamApplyNormalsJob : IJob // can't be IJobFor as we have to write to stream0 arbitrarily
    {
        [NativeDisableContainerSafetyRestriction][ReadOnly] private readonly NativeArray<TriangleUInt16> triangles;
        [ReadOnly] private readonly NativeArray<float3> faceNormals;
        
        // we both read and write from stream0
        private NativeArray<SingleStreams.Stream0> stream0;

        public SingleStreamApplyNormalsJob(NativeArray<TriangleUInt16> triangles,
            NativeArray<SingleStreams.Stream0> stream0, NativeArray<float3> faceNormals)
        {
            this.triangles = triangles;
            this.stream0 = stream0;
            this.faceNormals = faceNormals;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Apply(int vertexIndex, float3 normal)
        {
            SingleStreams.Stream0 vertex = stream0[vertexIndex];
            vertex.normal = normal;
            stream0[vertexIndex] = vertex;
        }
        
        public void Execute()
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                TriangleUInt16 triangle = triangles[i];
                float3 normal = faceNormals[i];
                Apply(triangle.a, normal);
                Apply(triangle.b, normal);
                Apply(triangle.c, normal);
            }
        }
    }
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct SingleStreamApplySmoothedNormalsJob : IJobFor
    {
        [ReadOnly] private readonly NativeArray<float3> faceNormals;
        [ReadOnly] private readonly NativeArray<VertexTris> vertexTris;
        
        private NativeArray<SingleStreams.Stream0> stream0;

        private static readonly float3 DEFAULT_NORMAL = new float3(0, 0, 1);
        
        public SingleStreamApplySmoothedNormalsJob(NativeArray<VertexTris> vertexTris,
            NativeArray<SingleStreams.Stream0> stream0, NativeArray<float3> faceNormals)
        {
            this.stream0 = stream0;
            this.faceNormals = faceNormals;
            this.vertexTris = vertexTris;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float3 SmoothedNormal(VertexTris tris)
        {
            if (tris.count == 0)
            {
                return DEFAULT_NORMAL;
            }
            
            float3 normalSum = faceNormals[tris.a];
            if (tris.count > 1)
            {
                normalSum += faceNormals[tris.b];
            }
            if (tris.count > 2)
            {
                normalSum += faceNormals[tris.c];
            }
            if (tris.count > 3)
            {
                normalSum += faceNormals[tris.d];
            }

            return normalSum / tris.count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Apply(int vertexIndex, float3 normal)
        {
            SingleStreams.Stream0 vertex = stream0[vertexIndex];
            vertex.normal = normal;
            stream0[vertexIndex] = vertex;
        }
        
        public void Execute(int index)
        {
            VertexTris tris = vertexTris[index];
            float3 smoothedNormal = SmoothedNormal(tris);
            Apply(index, smoothedNormal);
        }
    }
}