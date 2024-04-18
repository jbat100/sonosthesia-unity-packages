using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Sonosthesia.Mesh
{
    // source https://catlikecoding.com/unity/tutorials/procedural-meshes/
    
    public struct PointyHexagonGrid : IMeshGenerator
    {
        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
        {
            int vi = 7 * Resolution * z, ti = 6 * Resolution * z;
            
            float h = sqrt(3f) / 4f;
            float2 centerOffset = 0f;
            if (Resolution > 1) {
                centerOffset.x = (((z & 1) == 0 ? 0.5f : 1.5f) - Resolution) * h;
                centerOffset.y = -0.375f * (Resolution - 1);
            }

            for (int x = 1; x <= Resolution; x++, vi += 7, ti += 6)
            {
                float2 center = (float2(2f * h * x, 0.75f * z) + centerOffset) / Resolution;
                float2 xCoordinates = center.x + float2(-h, h) / Resolution;
                float4 zCoordinates = center.y + float4(-0.5f, -0.25f, 0.25f, 0.5f) / Resolution;
                
                Vertex vertex = new Vertex();
                vertex.texCoord0 = 0.5f;
                vertex.position.xz = center;
                vertex.normal.y = 1f;
                vertex.tangent.xw = float2(1f, -1f);
                streams.SetVertex(vi + 0, vertex);

                vertex.texCoord0.y = 0f;
                vertex.position.z = zCoordinates.x;
                streams.SetVertex(vi + 1, vertex);

                vertex.texCoord0 = float2(0.5f - h, 0.25f);
                vertex.position.x = xCoordinates.x;
                vertex.position.z = zCoordinates.y;
                streams.SetVertex(vi + 2, vertex);
                
                vertex.texCoord0 = float2(0.5f - h, 0.75f);
                vertex.position.z = zCoordinates.z;
                streams.SetVertex(vi + 3, vertex);
                
                vertex.texCoord0 = float2(0.5f, 1f);
                vertex.position.x = center.x;
                vertex.position.z = zCoordinates.w;
                streams.SetVertex(vi + 4, vertex);
                
                vertex.texCoord0 = float2(0.5f + h, 0.75f);
                vertex.position.x = xCoordinates.y;
                vertex.position.z = zCoordinates.z;
                streams.SetVertex(vi + 5, vertex);

                vertex.texCoord0.y = 0.25f;
                vertex.position.z = zCoordinates.y;
                streams.SetVertex(vi + 6, vertex);
                
                streams.SetTriangle(ti + 0, vi + int3(0,1,2));
                streams.SetTriangle(ti + 1, vi + int3(0,2,3));
                streams.SetTriangle(ti + 2, vi + int3(0,3,4));
                streams.SetTriangle(ti + 3, vi + int3(0,4,5));
                streams.SetTriangle(ti + 4, vi + int3(0,5,6));
                streams.SetTriangle(ti + 5, vi + int3(0,6,1));
            }
        }

        public int VertexCount => 7 * Resolution * Resolution;
        public int IndexCount => 18 * Resolution * Resolution;
        public int JobLength => Resolution;
        
        public int Resolution { get; set; }
        
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(
            (Resolution > 1 ? 0.5f + 0.25f / Resolution : 0.5f) * sqrt(3f),
            0f,
            0.75f + 0.25f / Resolution
        ));
    }
}