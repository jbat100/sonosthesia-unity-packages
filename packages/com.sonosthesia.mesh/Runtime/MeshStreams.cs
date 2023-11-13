using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public interface IMeshStreams
    {
        void Setup(UnityEngine.Mesh.MeshData data, Bounds bounds, int vertexCount, int indexCount);
        void SetVertex(int index, Vertex data);
        void SetTriangle(int index, int3 triangle);
    }
}