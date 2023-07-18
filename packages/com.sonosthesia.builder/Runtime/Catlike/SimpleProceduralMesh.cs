using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Builder
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SimpleProceduralMesh : MonoBehaviour
    {
        protected void OnEnable ()
        {
            Mesh mesh = SimpleReusedVertices();

            GetComponent<MeshFilter>().mesh = mesh;
        }
        
        private Mesh SimpleDoubledVertices() 
        {
            return new Mesh
            {
                name = "Procedural Mesh",
                vertices = new []
                {
                    Vector3.zero, Vector3.right, Vector3.up,
                    new Vector3(1.1f, 0f), new Vector3(0f, 1.1f), new Vector3(1.1f, 1.1f)
                },
                triangles = new [] { 0, 1, 2, 3, 4, 5 },
                normals = new []
                {
                    Vector3.back, Vector3.back, Vector3.back,
                    Vector3.back, Vector3.back, Vector3.back
                },
                uv = new []
                {
                    Vector2.zero, Vector2.right, Vector2.up,
                    Vector2.right, Vector2.up, Vector2.one
                },
                tangents = new []
                {
                    new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f),
                    new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f) 
                } 
            };
        }
        
        private Mesh SimpleReusedVertices() 
        {
            return new Mesh
            {
                name = "Procedural Mesh",
                vertices = new []
                {
                    Vector3.zero, Vector3.right, Vector3.up, new Vector3(1f, 1f)
                },
                triangles = new [] { 0, 1, 2, 1, 2, 3 },
                normals = new []
                {
                    Vector3.back, Vector3.back, Vector3.back, Vector3.back
                },
                uv = new []
                {
                    Vector2.zero, Vector2.right, Vector2.up, Vector2.one
                },
                tangents = new []
                {
                    new Vector4(1f, 0f, 0f, -1f), 
                    new Vector4(1f, 0f, 0f, -1f), 
                    new Vector4(1f, 0f, 0f, -1f),
                    new Vector4(1f, 0f, 0f, -1f)
                } 
            };
        }
    }
    
    
    
}
