using UnityEngine;

namespace Sonosthesia.Builder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class JobProceduralMesh : MonoBehaviour
    {

        public enum MaterialMode
        {
            Flat, 
            Ripple,
            LatLonMap,
            CubeMap
        }

        [SerializeField] private MaterialMode _materialMode;
        
        [SerializeField] private Material[] _materials;
        
        [System.Flags]
        public enum GizmoMode
        {
            Vertices = 1 << 1, 
            Normals = 1 << 2, 
            Tangents = 1 << 3
        }
        
        static readonly MeshJobScheduleDelegate[] _jobs = 
        {
            MeshJob<RowSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedTriangleGrid, SingleStreams>.ScheduleParallel,
            MeshJob<PointyHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<FlatHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<UVSphere, SingleStreams>.ScheduleParallel
        };

        public enum MeshType 
        {
            SquareGrid, 
            SharedSquareGrid, 
            SharedTriangleGrid, 
            PointyHexagonGrid, 
            FlatHexagonGrid, 
            UVSphere
        };

        [SerializeField] MeshType _meshType;

        [SerializeField, Range(1, 50)] int _resolution = 1;

        [SerializeField] private GizmoMode _gizmoMode;

        private Mesh _mesh;

        private Vector3[] _vertices, _normals;
        private Vector4[] _tangents;

        protected void Awake () 
        {
            _mesh = new Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        
        protected void OnValidate () => enabled = true;

        protected void Update () 
        {
            GetComponent<MeshRenderer>().material = _materials[(int)_materialMode];
            GenerateMesh();
            _vertices = null;
            _normals = null;
            _tangents = null;
            enabled = false;
        }
        
        protected void OnDrawGizmos () 
        {
            if (_mesh == null || _gizmoMode == 0) 
            {
                return;
            }
            Transform t = transform;
            
            bool drawVertices = (_gizmoMode & GizmoMode.Vertices) != 0;
            bool drawNormals = (_gizmoMode & GizmoMode.Normals) != 0;
            bool drawTangents = (_gizmoMode & GizmoMode.Tangents) != 0;
            
            _vertices ??= _mesh.vertices;
            if (drawNormals && _normals == null) 
            {
                _normals = _mesh.normals;
            }
            if (drawTangents && _tangents == null) 
            {
                _tangents = _mesh.tangents;
            }
            
            for (int i = 0; i < _vertices.Length; i++) 
            {
                Vector3 position = t.TransformPoint(_vertices[i]);
                if (drawVertices)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(position, 0.02f);
                }
                if (drawNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(position, t.TransformDirection(_normals[i] * 0.2f));
                }
                if (drawTangents)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(position, t.TransformDirection(_tangents[i] * 0.2f));   
                }
            }
        }

        private void GenerateMesh()
        {
            Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];
            _jobs[(int)_meshType](_mesh, meshData, _resolution, default).Complete();
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);
        }
    }
}