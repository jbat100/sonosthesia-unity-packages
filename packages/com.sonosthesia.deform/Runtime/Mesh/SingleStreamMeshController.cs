using System;
using Sonosthesia.Mesh;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Deform
{

    // all meshes use SingleStreams layout, which allows deforming subclasses to access the raw data 
    // directly, make optimizations and useful assumptions
    
    public abstract class SingleStreamMeshController : MeshController
    {
        public enum NormalCompute
        {
            None,
            Face,
            Smoothed
        }
        
        private static readonly int materialIsPlaneId = Shader.PropertyToID("_IsPlane");

        protected bool IsPlane => _meshType < MeshType.CubeSphere;

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
        
        private delegate void GetInfoDelegate(int resolution, bool supportVectorization, 
            out int vertexCount, out int indexCount, out Bounds bounds);
        
        static readonly GetInfoDelegate[] _infoJobs =
        {
            MeshJob<RowSquareGrid, SingleStreams>.GetInfo,
            MeshJob<SharedSquareGrid, SingleStreams>.GetInfo,
            MeshJob<SharedTriangleGrid, SingleStreams>.GetInfo,
            MeshJob<PointyHexagonGrid, SingleStreams>.GetInfo,
            MeshJob<FlatHexagonGrid, SingleStreams>.GetInfo,
            MeshJob<CubeSphere, SingleStreams>.GetInfo,
            MeshJob<SharedCubeSphere, SingleStreams>.GetInfo,
            MeshJob<IcoSphere, SingleStreams>.GetInfo,
            MeshJob<GeoIcoSphere, SingleStreams>.GetInfo,
            MeshJob<OctaSphere, SingleStreams>.GetInfo,
            MeshJob<GeoOctaSphere, SingleStreams>.GetInfo,
            MeshJob<UVSphere, SingleStreams>.GetInfo
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

        [SerializeField] private MeshType _meshType;

        // note : increasing the resolution higher creates issues due to the number of indices being higher than
        // TriangleUInt16 allows
        
        [SerializeField, Range(1, 80)] private int _resolution = 1;

        [SerializeField, Range(-1f, 1f)] private float _displacement = 0.5f;

        [SerializeField] private bool _cacheMesh;

        [SerializeField] private NormalCompute _normalCompute;

        private Material _material;
        private bool _setIsPlane;

        
        public struct CacheKey : IEquatable<CacheKey>
        {
            public MeshType meshType;
            public int resolution;

            public CacheKey(MeshType meshType, int resolution)
            {
                this.meshType = meshType;
                this.resolution = resolution;
            }

            public bool Equals(CacheKey other)
            {
                return meshType == other.meshType && resolution == other.resolution;
            }

            public override bool Equals(object obj)
            {
                if (obj is CacheKey other)
                {
                    return Equals(other);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(meshType, resolution);
            }

            public static bool operator ==(CacheKey left, CacheKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(CacheKey left, CacheKey right)
            {
                return !(left == right);
            }
        }
        
        // holds a cached version of the undeformed mesh data
        private class MeshBuildCache : IDisposable
        {
            public CacheKey key;
            public NativeArray<SingleStreams.Stream0> stream0;
            public NativeArray<TriangleUInt16> triangles;

            public void Dispose()
            {
                stream0.Dispose();
                triangles.Dispose();
            }

            public MeshBuildCache(CacheKey key,
                NativeArray<SingleStreams.Stream0> sourceStream0, NativeArray<TriangleUInt16> sourceTriangles)
            {
                this.key = key;
                stream0 = new NativeArray<SingleStreams.Stream0>(sourceStream0, Allocator.Persistent);
                triangles = new NativeArray<TriangleUInt16>(sourceTriangles, Allocator.Persistent);
            }
        }

        private MeshBuildCache _meshBuildCache;

        private class NormalComputeHelper : IDisposable
        {
            public readonly CacheKey key;
            public readonly NormalCompute compute;
            public readonly NativeArray<VertexTris> vertexTris;
            public readonly NativeArray<float3> faceNormals;
            public readonly NativeArray<float3> vertexNormals;

            public NormalComputeHelper(CacheKey key, NormalCompute compute, int vertexCount, int triangleCount)
            {
                this.key = key;
                this.compute = compute;

                if (compute == NormalCompute.Smoothed)
                {
                    vertexTris = new NativeArray<VertexTris>(vertexCount, Allocator.Persistent);    
                }

                if (compute != NormalCompute.None)
                {
                    faceNormals = new NativeArray<float3>(triangleCount, Allocator.Persistent);
                    vertexNormals = new NativeArray<float3>(vertexCount, Allocator.Persistent);   
                }
            }

            public void Dispose()
            {
                vertexTris.Dispose();
            }
        }

        private NormalComputeHelper _normalComputeHelper;

        private CacheKey MakeCacheKey() => new CacheKey(_meshType, _resolution);
        
        protected override void Awake()
        {
            base.Awake();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                _material = meshRenderer.material;
                _setIsPlane = _material.HasFloat(materialIsPlaneId);   
            }
        }

        protected override void Update()
        {
            base.Update();
            if (_setIsPlane)
            {
                _material.SetFloat(materialIsPlaneId, IsPlane ? 1f : 0f);
            }
        }

        protected virtual void OnDestroy()
        {
            _meshBuildCache?.Dispose();
            _meshBuildCache = null;
        }

        private JobHandle BuildMesh(UnityEngine.Mesh.MeshData data)
        {
            return _meshJobs[(int)_meshType](
                Mesh,
                data,
                _resolution,
                default,
                Vector3.one * Mathf.Abs(_displacement),
                true);
        }

        private void GetInfo(out int vertexCount, out int indexCount, out Bounds bounds)
        {
            _infoJobs[(int)_meshType](_resolution, true, out vertexCount, out indexCount, out bounds);
        }

        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data)
        {
            CacheKey currentCacheKey = MakeCacheKey();

            if (_cacheMesh)
            {
                if (_meshBuildCache == null || _meshBuildCache.key != currentCacheKey)
                {
                    BuildMesh(data).Complete();

                    _meshBuildCache?.Dispose();
                    _meshBuildCache = new MeshBuildCache (currentCacheKey,
                        data.GetVertexData<SingleStreams.Stream0>(),
                        data.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2)
                    );
                    
                    // Debug.Log($"{this} built and cached mesh data");

                }
                else
                {
                    SingleStreams singleStreams = default;
                    GetInfo(out int vertexCount, out int indexCount, out Bounds bounds);
                    singleStreams.Setup(data, bounds, vertexCount, indexCount);
                    singleStreams.Import(_meshBuildCache.stream0, _meshBuildCache.triangles);
                    
                    // Debug.Log($"{this} reusing cached mesh data");
                }
            }
            else
            {
                if (_meshBuildCache != null)
                {
                    _meshBuildCache.Dispose();
                    _meshBuildCache = null;
                }
                
                BuildMesh(data).Complete();
                
                // Debug.Log($"{this} built mesh data");
            }
            
            NativeArray<TriangleUInt16> triangles = data.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
            NativeArray<SingleStreams.Stream0> vertices = data.GetVertexData<SingleStreams.Stream0>();
            
            JobHandle vertexTrisJob = default;
            if (_normalComputeHelper == null || 
                _normalComputeHelper.key != currentCacheKey || 
                _normalComputeHelper.compute != _normalCompute)
            {
                _normalComputeHelper?.Dispose();
                
                _normalComputeHelper = new NormalComputeHelper(currentCacheKey, _normalCompute,
                    vertices.Length, triangles.Length);
               
                if (_normalCompute == NormalCompute.Smoothed)
                {
                    vertexTrisJob = new SingleStreamVertexTrisJob(triangles, _normalComputeHelper.vertexTris).Schedule();
                    vertexTrisJob.Complete();
                }
            }
            
            // we could schedule the mesh job in parallel with the deformation jobs but only if the deformation
            // array has the right size (i.e. mesh size has not changed since last update)
            
            JobHandle deformJob = DeformMesh(data, _resolution, _displacement, default);
            deformJob.Complete();
            
            // vertexTrisJob reads only from tris, deform job uses only verts, the two can run in parallel
            
            // JobHandle.CombineDependencies(vertexTrisJob, deformJob).Complete();

            // once the mesh deformation is complete we can compute the normals

            JobHandle ComputeFaceNormals(JobHandle dependency)
            {
                SingleStreamFaceNormalsJob job = new SingleStreamFaceNormalsJob(triangles, vertices, _normalComputeHelper.faceNormals);
                return job.ScheduleParallel(triangles.Length, 100, dependency);
            }

            JobHandle ApplyFaceNormals(JobHandle dependency)
            {
                SingleStreamApplyNormalsJob job = new SingleStreamApplyNormalsJob(triangles, vertices, _normalComputeHelper.faceNormals);
                return job.Schedule(dependency);
            }

            JobHandle ApplySmoothedFaceNormals(JobHandle dependency)
            {
                SingleStreamApplySmoothedNormalsJob job = new SingleStreamApplySmoothedNormalsJob(_normalComputeHelper.vertexTris, 
                    vertices, _normalComputeHelper.faceNormals);
                return job.ScheduleParallel(vertices.Length, 100, dependency);
            }

            switch (_normalCompute)
            {
                case NormalCompute.Face:
                {
                    ComputeFaceNormals(default).Complete();
                    ApplyFaceNormals(default).Complete();
                }
                    break;
                case NormalCompute.Smoothed:
                {
                    ComputeFaceNormals(default).Complete();
                    ApplySmoothedFaceNormals(default).Complete();
                }
                    break;
                default:
                    break;
            }
        }

        protected abstract JobHandle DeformMesh(UnityEngine.Mesh.MeshData data, int resolution, float displacement, JobHandle dependency);
    }
}