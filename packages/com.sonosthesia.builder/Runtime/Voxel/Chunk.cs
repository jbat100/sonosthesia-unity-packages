using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Builder
{
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private int3 _dimensions;

        private Block[,,] _blocks;
        
        protected void Start()
        {
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            mr.material = _material;
            
            _blocks = new Block[_dimensions.x, _dimensions.y, _dimensions.z];

            int vertexStart = 0;
            int triStart = 0;
            int meshCount = _dimensions.x * _dimensions.y * _dimensions.z;
            int m = 0;
            
            List<Mesh> inputMeshes = new List<Mesh>(meshCount);

            ProcessMeshDataJob job = new ProcessMeshDataJob()
            {
                VertexStarts = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory),
                TriStarts = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
            };

            for (int z = 0; z < _dimensions.z; z++)
            {
                for (int y = 0; y < _dimensions.y; y++)
                {
                    for (int x = 0; x < _dimensions.x; x++)
                    {
                        _blocks[x, y, z] = new Block(new Vector3(x, y, z), VoxelUtils.BlockType.DIRT);
                        inputMeshes.Add(_blocks[x, y, z].mesh);
                        int vcount = _blocks[x, y, z].mesh.vertexCount;
                        int icount = (int)_blocks[x, y, z].mesh.GetIndexCount(0);
                        job.VertexStarts[m] = vertexStart;
                        job.TriStarts[m] = triStart;
                        vertexStart += vcount;
                        triStart += icount;
                        m++;
                    }
                }
            }

            job.MeshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
            Mesh.MeshDataArray outputMeshData = Mesh.AllocateWritableMeshData(1);
            job.OutputMesh = outputMeshData[0];
            job.OutputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
            job.OutputMesh.SetVertexBufferParams(vertexStart, 
                new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

            JobHandle handle = job.Schedule(meshCount, 4);
            Mesh mesh = new Mesh();
            mesh.name = "Chunk";
            SubMeshDescriptor sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
            sm.firstVertex = 0;
            sm.vertexCount = vertexStart;
            
            handle.Complete();

            job.OutputMesh.subMeshCount = 1;
            job.OutputMesh.SetSubMesh(0, sm);
            
            Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new [] {mesh});

            job.MeshData.Dispose();
            job.VertexStarts.Dispose();
            job.TriStarts.Dispose();
            
            mesh.RecalculateBounds();

            mf.mesh = mesh;
        }

    }

    [BurstCompile]
    internal struct ProcessMeshDataJob : IJobParallelFor
    {
        [ReadOnly] public Mesh.MeshDataArray MeshData;
        public Mesh.MeshData OutputMesh;
        public NativeArray<int> VertexStarts;
        public NativeArray<int> TriStarts;

        public void Execute(int index)
        {
            Mesh.MeshData data = MeshData[index];
            int vCount = data.vertexCount;
            int vStart = VertexStarts[index];

            NativeArray<float3> verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());
            NativeArray<float3> normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(verts.Reinterpret<Vector3>());
            NativeArray<float3> uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, verts.Reinterpret<Vector3>());

            NativeArray<Vector3> outputVerts = data.GetVertexData<Vector3>();
            NativeArray<Vector3> outputNormals = data.GetVertexData<Vector3>(stream: 1);
            NativeArray<Vector3> outputUVs = data.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vCount; i++)
            {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
            }

            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            int tCount = data.GetSubMesh(0).indexCount;
            int tStart = TriStarts[index];
            NativeArray<int> outputTris = OutputMesh.GetIndexData<int>();

            if (data.indexFormat == IndexFormat.UInt16)
            {
                NativeArray<ushort> tris = data.GetIndexData<ushort>();
                for (int i = 0; i < tCount; i++)
                {
                    outputTris[i + tStart] = vStart + tris[i];
                }
            }
            else
            {
                NativeArray<int> tris = data.GetIndexData<int>();
                for (int i = 0; i < tCount; i++)
                {
                    outputTris[i + tStart] = vStart + tris[i];
                }
            }
        }
    }
}