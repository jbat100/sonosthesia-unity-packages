using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public struct SquareGrid : IMeshGenerator
    {
        public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
        {
            int vi = 4 * i, ti = 2 * i;
            
            int z = i / Resolution;
            int x = i - Resolution * z;
            
            float4 coordinates = float4(x, x + 0.9f, z, z + 0.9f) / Resolution - 0.5f;;
            
            Vertex vertex = new Vertex();
            vertex.position.xz = coordinates.xz;
            vertex.normal.y = 1f;
            vertex.tangent.xw = float2(1f, -1f);
            streams.SetVertex(vi + 0, vertex);
            
            vertex.position.xz = coordinates.yz;
            vertex.texCoord0 = float2(1f, 0f);
            streams.SetVertex(vi + 1, vertex);

            vertex.position.xz = coordinates.xw;
            vertex.texCoord0 = float2(0f, 1f);
            streams.SetVertex(vi + 2, vertex);

            vertex.position.xz = coordinates.yw;
            vertex.texCoord0 = 1f;
            streams.SetVertex(vi + 3, vertex);
            
            streams.SetTriangle(ti + 0, vi + int3(0, 2, 1));
            streams.SetTriangle(ti + 1, vi + int3(1, 2, 3));
        }

        public int VertexCount => 4 * Resolution * Resolution;
        public int IndexCount => 6 * Resolution * Resolution;
        public int JobLength => Resolution;
        
        public int Resolution { get; set; }
        
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f, 0f, 1f));
    }
}