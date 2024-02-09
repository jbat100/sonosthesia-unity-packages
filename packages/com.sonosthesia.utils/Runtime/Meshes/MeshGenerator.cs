using UnityEngine;

namespace Sonosthesia.Utils
{
    /// <summary>
    /// For simple meshes like quads and cubes, inefficient
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public abstract class MeshGenerator : MonoBehaviour
    {
        private MeshFilter _meshFilter;

        protected virtual void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }
        
        protected virtual void OnEnable() => RefreshMesh();

        protected virtual void OnValidate() => RefreshMesh();

        private void RefreshMesh()
        {
            _meshFilter = _meshFilter ? _meshFilter : GetComponent<MeshFilter>();
            if (_meshFilter)
            {
                _meshFilter.mesh = GenerateMesh();
            }
        }

        protected abstract Mesh GenerateMesh();
    }

    internal static class MeshGeneratorHelper
    {
        internal static Mesh GenerateQuad(Vector2 xRange, Vector2 yRange, Vector2 uRange, Vector2 vRange)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new (xRange.x, yRange.x, 0),
                new (xRange.y, yRange.x, 0),
                new (xRange.x, yRange.y, 0),
                new (xRange.y, yRange.y, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new (uRange.x, vRange.x),
                new (uRange.y, vRange.x),
                new (uRange.x, vRange.y),
                new (uRange.y, vRange.y)
            };
            mesh.uv = uv;

            return mesh;
        }
    }
}