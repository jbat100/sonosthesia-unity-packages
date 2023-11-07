using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;
using Random = UnityEngine.Random;

namespace Sonosthesia.Builder
{
    public class JobFractal : MonoBehaviour    
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct UpdateFractalLevelJob : IJobFor
        {
            public float scale;
            public float deltaTime;

            [ReadOnly]
            public NativeArray<FractalPart> parents;
            
            public NativeArray<FractalPart> parts;

            [WriteOnly]
            public NativeArray<float3x4> matrices;

            public void Execute(int i)
            {
                FractalPart parent = parents[i / 5];
                FractalPart part = parts[i];
                part.spinAngle += part.spinVelocity * deltaTime;
                
                float3 upAxis = mul(mul(parent.worldRotation, part.rotation), up());
                float3 sagAxis = cross(up(), upAxis);
                sagAxis = normalize(sagAxis);
                
                float sagMagnitude = length(sagAxis);
                quaternion baseRotation;
                if (sagMagnitude > 0f) {
                    sagAxis /= sagMagnitude;
                    quaternion sagRotation = quaternion.AxisAngle(sagAxis, part.maxSagAngle * sagMagnitude);
                    baseRotation = mul(sagRotation, parent.worldRotation);
                }
                else {
                    baseRotation = parent.worldRotation;
                }

                part.worldRotation = mul(baseRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));
                part.worldPosition = parent.worldPosition + mul(part.worldRotation, float3(0f, 1.5f * scale, 0f));;
                parts[i] = part;
                float3x3 r = float3x3(part.worldRotation) * scale;
                matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
            }
        }
        
        private static MaterialPropertyBlock _propertyBlock;
        
        private static readonly int 
            _colorAId = Shader.PropertyToID("_ColorA"),
            _colorBId = Shader.PropertyToID("_ColorB"),
            _matricesId = Shader.PropertyToID("_Matrices"),
            _sequenceNumbersId = Shader.PropertyToID("_SequenceNumbers");
        
        [SerializeField, Range(3, 9)] private int _depth = 4;

        [SerializeField] private Mesh _mesh;
        [SerializeField] private Mesh _leafMesh;

        [SerializeField] private Material _material;
        
        [SerializeField] private Gradient _gradientA, _gradientB;
        
        [SerializeField] private Color _leafColorA, _leafColorB;
        
        [SerializeField, Range(0f, 90f)] private float _maxSagAngleA = 15f, _maxSagAngleB = 25f;
        
        [SerializeField, Range(0f, 90f)] private float _spinSpeedA = 20f, _spinSpeedB = 25f;

        [SerializeField, Range(0f, 1f)] private float _reverseSpinChance = 0.25f;

        private static readonly float3[] _directions = {
            up(), right(), left(), forward(), back()
        };

        private static readonly quaternion[] _rotations = {
            quaternion.identity,
            quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI),
            quaternion.RotateX(0.5f * PI), quaternion.RotateX(-0.5f * PI)
        };
        
        struct FractalPart 
        {
            public float3 direction, worldPosition;
            public quaternion rotation, worldRotation;
            public float maxSagAngle, spinAngle, spinVelocity;
        }
        
        private NativeArray<FractalPart>[] _parts;
        private NativeArray<float3x4>[] _matrices;
        private ComputeBuffer[] _matricesBuffers;
        private Vector4[] _sequenceNumbers;
        
        protected void OnValidate () 
        {
            if (_parts != null && enabled) 
            {
                OnDisable();
                OnEnable();
            }
        }
        
        protected void OnEnable () 
        {
            _propertyBlock ??= new MaterialPropertyBlock();
            
            _parts = new NativeArray<FractalPart>[_depth];
            _matrices = new NativeArray<float3x4>[_depth];
            _matricesBuffers = new ComputeBuffer[_depth];
            _sequenceNumbers = new Vector4[_depth];
            int stride = 12 * 4;
            for (int i = 0, length = 1; i < _parts.Length; i++, length *= 5) 
            {
                _parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                _matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
                _matricesBuffers[i] = new ComputeBuffer(length, stride);
                _sequenceNumbers[i] = new Vector4(Random.value, Random.value, Random.value, Random.value);
            }
            _parts[0][0] = CreatePart(0);
            
            for (int li = 1; li < _parts.Length; li++) 
            {
                NativeArray<FractalPart> levelParts = _parts[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi += 5) 
                {
                    for (int ci = 0; ci < 5; ci++) 
                    {
                        levelParts[fpi + ci] = CreatePart(ci);
                    }
                }
            }
        }
        
        protected void OnDisable () 
        {
            for (int i = 0; i < _matricesBuffers.Length; i++) 
            {
                _matricesBuffers[i].Release();
                _parts[i].Dispose();
                _matrices[i].Dispose();
            }
            _parts = null;
            _matrices = null;
            _matricesBuffers = null;
            _sequenceNumbers = null;
        }
        
        protected void Update () 
        {
            float deltaTime = Time.deltaTime;
            int leafIndex = _matricesBuffers.Length - 1;

            FractalPart rootPart = _parts[0][0];
            rootPart.spinAngle += rootPart.spinVelocity * deltaTime;
            rootPart.worldRotation = mul(transform.rotation, mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle)));
            _parts[0][0] = rootPart;
            float objectScale = transform.lossyScale.x;
            float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
            _matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);
            
            float scale = 1f;
            JobHandle jobHandle = default;
            for (int li = 1; li < _parts.Length; li++) 
            {
                scale *= 0.5f;
                jobHandle = new UpdateFractalLevelJob {
                    deltaTime = deltaTime,
                    scale = scale,
                    parents = _parts[li - 1],
                    parts = _parts[li],
                    matrices = _matrices[li]
                }.ScheduleParallel(_parts[li].Length, 5, jobHandle);
            }
            jobHandle.Complete();

            var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
            for (int i = 0; i < _matricesBuffers.Length; i++) 
            {
                Color colorA, colorB;
                Mesh instanceMesh;
                if (i == leafIndex) 
                {
                    colorA = _leafColorA;
                    colorB = _leafColorB;
                    instanceMesh = _leafMesh;
                }
                else 
                {
                    float gradientInterpolator = i / (_matricesBuffers.Length - 2f);
                    colorA = _gradientA.Evaluate(gradientInterpolator);
                    colorB = _gradientB.Evaluate(gradientInterpolator);
                    instanceMesh = _mesh;
                }
                ComputeBuffer buffer = _matricesBuffers[i];
                buffer.SetData(_matrices[i]);   
                _propertyBlock.SetColor(_colorAId, colorA);
                _propertyBlock.SetColor(_colorBId, colorB);
                _propertyBlock.SetBuffer(_matricesId, buffer);
                _propertyBlock.SetVector(_sequenceNumbersId, _sequenceNumbers[i]);
                Graphics.DrawMeshInstancedProcedural(instanceMesh, 0, _material, bounds, buffer.count, _propertyBlock);
            }
        }
        
        private FractalPart CreatePart (int childIndex) 
        {
            return new FractalPart() {
                direction = _directions[childIndex],
                rotation = _rotations[childIndex],
                maxSagAngle = radians(Random.Range(_maxSagAngleA, _maxSagAngleB)),
                spinVelocity = (Random.value < _reverseSpinChance ? -1f : 1f) * radians(Random.Range(_spinSpeedA, _spinSpeedB))
            };
        }
    }
}