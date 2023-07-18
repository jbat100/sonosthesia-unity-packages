using UnityEngine;

namespace Sonosthesia.Builder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class JobProceduralMesh : MonoBehaviour
    {
        static readonly MeshJobScheduleDelegate[] _jobs = {
            MeshJob<RowSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedTriangleGrid, SingleStreams>.ScheduleParallel,
            MeshJob<PointyHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<FlatHexagonGrid, SingleStreams>.ScheduleParallel
        };

        public enum MeshType {
            SquareGrid, SharedSquareGrid, SharedTriangleGrid, PointyHexagonGrid, FlatHexagonGrid
        };

        [SerializeField] MeshType _meshType;

        [SerializeField, Range(1, 50)] int _resolution = 1;
        
        private Mesh _mesh;

        protected void Awake () 
        {
            _mesh = new Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        
        protected void OnValidate () => enabled = true;

        protected void Update () 
        {
            GenerateMesh();
            enabled = false;
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