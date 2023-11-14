using System;
using Sonosthesia.Mesh;
using Sonosthesia.Noise;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Deform
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProceduralSurface : MonoBehaviour
    {
        private static int materialIsPlaneId = Shader.PropertyToID("_IsPlane");
        
        bool IsPlane => _meshType < MeshType.CubeSphere;
        
        [Flags]
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
        
        [Flags]
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
                SurfaceJob<Lattice1D<Perlin, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Perlin, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Perlin, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Lattice1D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Lattice1D<Value, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice2D<Value, LatticeNormal>>.ScheduleParallel,
                SurfaceJob<Lattice3D<Value, LatticeNormal>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Simplex>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Simplex>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Simplex>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel
            },
            {
                SurfaceJob<Simplex1D<Value>>.ScheduleParallel,
                SurfaceJob<Simplex2D<Value>>.ScheduleParallel,
                SurfaceJob<Simplex3D<Value>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel
            },
            {
                SurfaceJob<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                SurfaceJob<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel
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
        
        static FlowJobScheduleDelegate[,] _flowJobs = {
            {
                FlowJob<Lattice1D<Perlin, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice2D<Perlin, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice3D<Perlin, LatticeNormal>>.ScheduleParallel
            },
            {
                FlowJob<Lattice1D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice2D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice3D<Smoothstep<Turbulence<Perlin>>, LatticeNormal>>.ScheduleParallel
            },
            {
                FlowJob<Lattice1D<Value, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice2D<Value, LatticeNormal>>.ScheduleParallel,
                FlowJob<Lattice3D<Value, LatticeNormal>>.ScheduleParallel
            },
            {
                FlowJob<Simplex1D<Simplex>>.ScheduleParallel,
                FlowJob<Simplex2D<Simplex>>.ScheduleParallel,
                FlowJob<Simplex3D<Simplex>>.ScheduleParallel
            },
            {
                FlowJob<Simplex1D<Turbulence<Simplex>>>.ScheduleParallel,
                FlowJob<Simplex2D<Turbulence<Simplex>>>.ScheduleParallel,
                FlowJob<Simplex3D<Turbulence<Simplex>>>.ScheduleParallel
            },
            {
                FlowJob<Simplex1D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                FlowJob<Simplex2D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel,
                FlowJob<Simplex3D<Smoothstep<Turbulence<Simplex>>>>.ScheduleParallel
            },
            {
                FlowJob<Simplex1D<Value>>.ScheduleParallel,
                FlowJob<Simplex2D<Value>>.ScheduleParallel,
                FlowJob<Simplex3D<Value>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, SmoothWorley, F1>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, SmoothWorley, F2>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel
            },
            {
                FlowJob<Voronoi1D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FlowJob<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
                FlowJob<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel
            }
        };

        [SerializeField] private NoiseType _noiseType;

        [SerializeField, Range(1, 3)] private int _dimensions = 1;

        [SerializeField] private FractalNoiseSettings _noiseSettings = FractalNoiseSettings.Default;

        [SerializeField] private int _seed;

        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField, Range(-1f, 1f)] private float _displacement = 0.5f;
        
        [SerializeField] private bool _recalculateNormals, _recalculateTangents;

        public enum FlowMode
        {
            Off, 
            Curl, 
            Downhill
        }

        [SerializeField] private FlowMode _flowMode;
        
        private ParticleSystem _flowSystem;
        private UnityEngine.Mesh _mesh;

        [NonSerialized] private Vector3[] _vertices, _normals;
        [NonSerialized] private Vector4[] _tangents;
        [NonSerialized] private int[] _triangles;

        protected void Awake () 
        {
            _materials[(int)MaterialMode.Displacement] = new Material(_materials[(int)MaterialMode.Displacement]);
            _mesh = new UnityEngine.Mesh { name = "Procedural Mesh" };
            _flowSystem = GetComponent<ParticleSystem>();
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
                    materialIsPlaneId, IsPlane ? 1f : 0f
                );
            }
            
            if (_flowMode == FlowMode.Off) 
            {
                _flowSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                _flowSystem.Play();
                ParticleSystem.ShapeModule shapeModule = _flowSystem.shape;
                shapeModule.shapeType = IsPlane ? ParticleSystemShapeType.Rectangle : ParticleSystemShapeType.Sphere;
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
        
        protected void OnParticleUpdateJobScheduled () 
        {
            if (_flowMode != FlowMode.Off)
            {
                _flowJobs[(int)_noiseType, _dimensions - 1](
                    _flowSystem, _noiseSettings, _seed, _domain, _displacement, IsPlane, _flowMode == FlowMode.Curl
                );
            }
        }

        private void GenerateMesh()
        {
            Debug.Log($"{this} {nameof(GenerateMesh)} with {nameof(_resolution)} {_resolution}");
            UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AllocateWritableMeshData(1);
            UnityEngine.Mesh.MeshData meshData = meshDataArray[0];
            
            _surfaceJobs[(int)_noiseType, _dimensions - 1](
                meshData, 
                _resolution, 
                _noiseSettings, 
                _seed,
                _domain,
                _displacement,
                IsPlane,
                _meshJobs[(int)_meshType](_mesh, meshData, _resolution, default, Vector3.one * Mathf.Abs(_displacement), true))
                .Complete();
            
            UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, _mesh);

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