using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Builder
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProceduralSurface : MonoBehaviour
    {
        private static int materialIsPlaneId = Shader.PropertyToID("_IsPlane");
        
        [System.Flags]
        public enum MeshOptimizationMode 
        {
            ReorderIndices = 1 << 0, 
            ReorderVertices = 1 << 1
        }

        [SerializeField] private MeshOptimizationMode _meshOptimization;
        
        public enum MaterialMode
        {
            Displacement,
            Flat,
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
            Tangents = 1 << 3,
            Triangles = 1 << 4
        }
        
        // changed to use only SingleStreams to simplify deformation code
        
        static readonly AdvancedMeshJobScheduleDelegate[] _meshJobs = 
        {
            MeshJob<RowSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedSquareGrid, SingleStreams>.ScheduleParallel,
            MeshJob<SharedTriangleGrid, SingleStreams>.ScheduleParallel,
            MeshJob<PointyHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<FlatHexagonGrid, SingleStreams>.ScheduleParallel,
            MeshJob<CubeSphere, SingleStreams>.ScheduleParallel,
            MeshJob<SharedCubeSphere, SingleStreams>.ScheduleParallel,
            MeshJob<IcoSphere, SingleStreams>.ScheduleParallel,
            MeshJob<GeoIcoSphere, SingleStreams>.ScheduleParallel,
            MeshJob<OctaSphere, SingleStreams>.ScheduleParallel,
            MeshJob<GeoOctaSphere, SingleStreams>.ScheduleParallel,
            MeshJob<UVSphere, SingleStreams>.ScheduleParallel
        };

        public enum MeshType 
        {
            SquareGrid, 
            SharedSquareGrid, 
            SharedTriangleGrid, 
            PointyHexagonGrid, 
            FlatHexagonGrid, 
            CubeSphere,
            SharedCubeSphere,
            Icosphere,
            GeoIcoSphere,
            OctaSphere,
            GeoOctaSphere,
            UVSphere
        };

        [SerializeField] MeshType _meshType;

        [SerializeField, Range(1, 50)] int _resolution = 1;

        [SerializeField] private GizmoMode _gizmoMode;
        
        static SurfaceJobScheduleDelegate[,] _surfaceJobs = {
            {
                SurfaceJob<Noise.Lattice1D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Perlin, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Lattice1D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Smoothstep<Noise.Turbulence<Noise.Perlin>>, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Lattice1D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice2D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Noise.Lattice3D<Noise.Value, Noise.LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Simplex>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Simplex>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Simplex>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Turbulence<Noise.Simplex>>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Smoothstep<Noise.Turbulence<Noise.Simplex>>>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Simplex1D<Noise.Value>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex2D<Noise.Value>>.ScheduleParallel,
                SurfaceJob<Noise.Simplex3D<Noise.Value>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Worley, Noise.F2MinusF1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.SmoothWorley, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Noise.Voronoi1D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi2D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Noise.Voronoi3D<Noise.LatticeNormal, Noise.Chebyshev, Noise.F2MinusF1>>.ScheduleParallel
            }
        };

        public enum NoiseType 
        {
            Perlin, PerlinSmoothTurbulence, PerlinValue, 
            Simplex, SimplexTurbulence, SimplexSmoothTurbulence, SimplexValue,
            VoronoiWorleyF1, VoronoiWorleyF2, VoronoiWorleyF2MinusF1, 
            VoronoiWorleySmoothLSE, VoronoiWorleySmoothPoly,
            VoronoiChebyshevF1, VoronoiChebyshevF2, VoronoiChebyshevF2MinusF1
        }

        [SerializeField] NoiseType _noiseType;

        [SerializeField, Range(1, 3)] int _dimensions = 1;

        [SerializeField] Noise.Settings _noiseSettings = Noise.Settings.Default;

        [SerializeField] SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField, Range(-1f, 1f)] float _displacement = 0.5f;
        
        [SerializeField] bool _recalculateNormals, _recalculateTangents;

        private Mesh _mesh;

        [NonSerialized] private Vector3[] _vertices, _normals;
        [NonSerialized] private Vector4[] _tangents;
        [NonSerialized] private int[] _triangles;

        protected void Awake () 
        {
            _materials[(int)MaterialMode.Displacement] = new Material(_materials[(int)MaterialMode.Displacement]);
            _mesh = new Mesh { name = "Procedural Mesh" };
            GetComponent<MeshFilter>().mesh = _mesh;
        }
        
        protected void OnValidate () => enabled = true;

        protected void Update () 
        {
            GenerateMesh();
            _vertices = null;
            _normals = null;
            _tangents = null;
            _triangles = null;
            enabled = false;
            
            if (_materialMode == MaterialMode.Displacement) 
            {
                _materials[(int)MaterialMode.Displacement].SetFloat(
                    materialIsPlaneId, _meshType < MeshType.CubeSphere ? 1f : 0f
                );
            }
            
            GetComponent<MeshRenderer>().material = _materials[(int)_materialMode];
        }
        
        protected void OnDrawGizmos () 
        {
            if (_mesh == null || _gizmoMode == 0) 
            {
                return;
            }
            Transform t = transform;
            
            bool drawVertices = (_gizmoMode & GizmoMode.Vertices) != 0;
            bool drawNormals = (_gizmoMode & GizmoMode.Normals) != 0 && _mesh.HasVertexAttribute(VertexAttribute.Normal);
            bool drawTangents = (_gizmoMode & GizmoMode.Tangents) != 0 && _mesh.HasVertexAttribute(VertexAttribute.Tangent);
            bool drawTriangles = (_gizmoMode & GizmoMode.Triangles) != 0;
            
            _vertices ??= _mesh.vertices;
            if (drawNormals && _normals == null) 
            {
                _normals = _mesh.normals;
            }
            if (drawTangents && _tangents == null) 
            {
                _tangents = _mesh.tangents;
            }
            if (drawTriangles && _triangles == null) 
            {
                _triangles = _mesh.triangles;
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
            
            if (drawTriangles) 
            {
                float colorStep = 1f / (_triangles.Length - 3);
                for (int i = 0; i < _triangles.Length; i += 3) 
                {
                    float c = i * colorStep;
                    Gizmos.color = new Color(c, 0f, c);
                    Gizmos.DrawSphere(
                        t.TransformPoint((
                            _vertices[_triangles[i]] +
                            _vertices[_triangles[i + 1]] +
                            _vertices[_triangles[i + 2]]
                        ) * (1f / 3f)),
                        0.02f
                    );
                }
            }
        }

        private void GenerateMesh()
        {
            Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
            Mesh.MeshData meshData = meshDataArray[0];
            
            _surfaceJobs[(int)_noiseType, _dimensions - 1](
                meshData, 
                _resolution, 
                _noiseSettings, 
                _domain,
                _displacement,
                _meshType < MeshType.CubeSphere,
                _meshJobs[(int)_meshType](_mesh, meshData, _resolution, default, Vector3.one * Mathf.Abs(_displacement), true))
                .Complete();
            
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);

            if (_recalculateNormals)
            {
                _mesh.RecalculateNormals();
            }

            if (_recalculateTangents)
            {
                _mesh.RecalculateTangents();                
            }

            switch (_meshOptimization)
            {
                case MeshOptimizationMode.ReorderIndices:
                    _mesh.OptimizeIndexBuffers();
                    break;
                case MeshOptimizationMode.ReorderVertices:
                    _mesh.OptimizeReorderVertexBuffer();
                    break;
                default:
                {
                    if (_meshOptimization != 0) 
                    {
                        _mesh.Optimize();
                    }
                    break;
                }
            }
        }
    }
}