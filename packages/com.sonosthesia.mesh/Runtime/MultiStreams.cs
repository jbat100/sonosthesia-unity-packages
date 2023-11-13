using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Mesh
{
    public struct MultiStreams : IMeshStreams
    {
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float3> _stream0, _stream1;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float4> _stream2;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float2> _stream3;
        
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<TriangleUInt16> _triangles;

        public void Setup(UnityEngine.Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount)
        {
            NativeArray<VertexAttributeDescriptor> descriptor = new NativeArray<VertexAttributeDescriptor>(
                4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
            );
            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(
                VertexAttribute.Normal, dimension: 3, stream: 1
            );
            descriptor[2] = new VertexAttributeDescriptor(
                VertexAttribute.Tangent, dimension: 4, stream: 2
            );
            descriptor[3] = new VertexAttributeDescriptor(
                VertexAttribute.TexCoord0, dimension: 2, stream: 3
            );
            meshData.SetVertexBufferParams(vertexCount, descriptor);
            descriptor.Dispose();

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt16);
			
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount)
                {
                    bounds = bounds,
                    vertexCount = vertexCount
                },
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            
            _stream0 = meshData.GetVertexData<float3>();
            _stream1 = meshData.GetVertexData<float3>(1);
            _stream2 = meshData.GetVertexData<float4>(2);
            _stream3 = meshData.GetVertexData<float2>(3);
            _triangles = meshData.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex(int index, Vertex vertex)
        {
            _stream0[index] = vertex.position;
            _stream1[index] = vertex.normal;
            _stream2[index] = vertex.tangent;
            _stream3[index] = vertex.texCoord0;
        }

        public void SetTriangle(int index, int3 triangle)
        {
            _triangles[index] = triangle;
        }
    }
}