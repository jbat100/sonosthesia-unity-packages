using UnityEngine;

namespace Sonosthesia.Scaffold
{
    [ExecuteAlways, RequireComponent(typeof(MeshFilter))]
    public class BiSplineBox : BiSplineAffordance
    {
        [SerializeField] private float _topOffset;

        [SerializeField] private float _bottomOffset;

        private MeshFilter _meshFilter;

        protected override void Setup()
        {
            _meshFilter = GetComponent<MeshFilter>();
            base.Setup();
        }

        protected override void RefreshAffordance()
        {
            Mesh mesh = new Mesh();

            BiSplineVertices splineVertices = Configuration.Vertices;
            
            Debug.Log($"Updating mesh with {nameof(BiSplineVertices)} {splineVertices}");
            
            Vector3 normal = splineVertices.Plane.normal;

            Vector3[] c = new Vector3[8];

            Vector3 bottomDisplacement = _bottomOffset * normal;
            c[0] = splineVertices.P0 + bottomDisplacement;
            c[1] = splineVertices.P1 + bottomDisplacement;
            c[2] = splineVertices.P2 + bottomDisplacement;
            c[3] = splineVertices.P3 + bottomDisplacement;

            Vector3 topDisplacement = _topOffset * normal;
            c[4] = splineVertices.P0 + topDisplacement;
            c[5] = splineVertices.P1 + topDisplacement;
            c[6] = splineVertices.P2 + topDisplacement;
            c[7] = splineVertices.P3 + topDisplacement;
            
            Debug.Log($"Calculated {nameof(topDisplacement)} {topDisplacement} {nameof(topDisplacement)} {topDisplacement}");
            
            // https://gist.github.com/prucha/866b9535d525adc984c4fe883e73a6c7
            
            Vector3[] vertices = new Vector3[]
            {
                c[0], c[1], c[2], c[3], // Bottom
                c[7], c[4], c[0], c[3], // Left
                c[4], c[5], c[1], c[0], // Front
                c[6], c[7], c[3], c[2], // Back
                c[5], c[6], c[2], c[1], // Right
                c[7], c[6], c[5], c[4]  // Top
            };

            //Vector3 down = -normal;
            //Vector3 up = normal;
            //Vector3 forward = Vector3.Cross(p1 - p0, normal);
            //Vector3 back = Vector3.Cross(p2 - p3, normal);
            //Vector3 left = Vector3.Cross(p3 - p0, normal);
            //Vector3 right = -left;
            
            Vector3 forward = -normal;
            Vector3 back = normal;
            Vector3 down = Vector3.Cross(splineVertices.P1 - splineVertices.P0, normal);
            Vector3 up = Vector3.Cross(splineVertices.P2 - splineVertices.P3, normal);
            Vector3 left = -Vector3.Cross(splineVertices.P3 - splineVertices.P0, normal);
            Vector3 right = -left;
            
            Vector3[] normals = new Vector3[]
            {
                down, down, down, down,             // Bottom
                left, left, left, left,             // Left
                forward, forward, forward, forward,	// Front
                back, back, back, back,             // Back
                right, right, right, right,         // Right
                up, up, up, up	                    // Top
            };
            
            
            Vector2 uv00 = new Vector2(0f, 0f);
            Vector2 uv10 = new Vector2(1f, 0f);
            Vector2 uv01 = new Vector2(0f, 1f);
            Vector2 uv11 = new Vector2(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
                uv11, uv01, uv00, uv10, // Bottom
                uv11, uv01, uv00, uv10, // Left
                uv11, uv01, uv00, uv10, // Front
                uv11, uv01, uv00, uv10, // Back	        
                uv11, uv01, uv00, uv10, // Right 
                uv11, uv01, uv00, uv10  // Top
            };
            
            int[] triangles = new int[]
            {
                3, 1, 0,        3, 2, 1,        // Bottom	
                7, 5, 4,        7, 6, 5,        // Left
                11, 9, 8,       11, 10, 9,      // Front
                15, 13, 12,     15, 14, 13,     // Back
                19, 17, 16,     19, 18, 17,	    // Right
                23, 21, 20,     23, 22, 21,	    // Top
            };
            
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            //mesh.normals = normals;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            
            //mesh.Optimize();

            _meshFilter.mesh = mesh;

        }
        
    }    
}


