using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Sonosthesia.Noise
{
    public abstract class Visualisation : MonoBehaviour
    {
        protected enum Shape
        {
            Plane, 
            UVSphere, 
            OctaSphere,
            Torus
        }

        private static readonly Shapes.ScheduleDelegate[] _shapeJobs = {
            Shapes.Job<Shapes.Plane>.ScheduleParallel,
            Shapes.Job<Shapes.UVSphere>.ScheduleParallel,
            Shapes.Job<Shapes.OctaSphere>.ScheduleParallel,
            Shapes.Job<Shapes.Torus>.ScheduleParallel
        };
        
        private static readonly int
            _positionsId = Shader.PropertyToID("_Positions"),
            _normalsId = Shader.PropertyToID("_Normals"),
            _configId = Shader.PropertyToID("_Config");
        
        [SerializeField] private Shape _shape;
        
        [SerializeField] private Mesh _instanceMesh;

        [SerializeField] private Material _material;

        [SerializeField, Range(1, 512)] private int _resolution = 16;

        [SerializeField, Range(-0.5f, 0.5f)] private float _displacement = 0.1f;
        
        [SerializeField, Range(0.1f, 10f)] float _instanceScale = 2f;

        private NativeArray<float3x4> _positions, _normals;

        private ComputeBuffer _positionsBuffer, _normalsBuffer;

        private MaterialPropertyBlock _propertyBlock;

        private bool _isDirty;
        private Bounds _bounds;
        
        protected abstract void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock);

        protected abstract void UpdateVisualization (NativeArray<float3x4> positions, int resolution, JobHandle handle);
        
        protected abstract void DisableVisualization();
        
        protected void OnEnable()
        {
            _isDirty = true;
            
            int length = _resolution * _resolution;
            length = length / 4 + (length & 1);
            
            _positions = new NativeArray<float3x4>(length, Allocator.Persistent);
            _positionsBuffer = new ComputeBuffer(length * 4, 12);
            
            _normals = new NativeArray<float3x4>(length, Allocator.Persistent);
            _normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);

            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.SetBuffer(_positionsId, _positionsBuffer);
            _propertyBlock.SetBuffer(_normalsId, _normalsBuffer);
            _propertyBlock.SetVector(_configId, new Vector4(_resolution, _instanceScale / _resolution, _displacement));
            
            EnableVisualization(length, _propertyBlock);
            
        }
        
        protected void OnDisable () 
        {
            DisableVisualization();
            
            _positions.Dispose();
            _positionsBuffer.Release();
            _positionsBuffer = null;

            _normals.Dispose();
            _normalsBuffer.Release();
            _normalsBuffer = null;
        }

        protected void OnValidate () 
        {
            if (_positionsBuffer != null && enabled) 
            {
                OnDisable();
                OnEnable();
            }
        }
        
        protected void Update () 
        {
            if (_isDirty || transform.hasChanged) 
            {
                _isDirty = false;
                transform.hasChanged = false;

                UpdateVisualization(
                    _positions, _resolution,
                    _shapeJobs[(int)_shape](
                        _positions, _normals, _resolution, transform.localToWorldMatrix, default
                    )
                );

                _positionsBuffer.SetData(_positions.Reinterpret<float3>(3 * 4 * 4));
                _normalsBuffer.SetData(_normals.Reinterpret<float3>(3 * 4 * 4));
                
                _bounds = new Bounds(
                    transform.position,
                    float3(2f * cmax(abs(transform.lossyScale)) + _displacement)
                );
            }
            
            Graphics.DrawMeshInstancedProcedural(
                _instanceMesh, 0, _material, _bounds,
                _resolution * _resolution, _propertyBlock
            );
        }
    }
}