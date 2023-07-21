using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class HashVisualisation : MonoBehaviour
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct HashJob : IJobFor {

            [WriteOnly]
            public NativeArray<uint> hashes;
        
            public void Execute(int i) 
            {
                hashes[i] = (uint)i;
            }
        }
        
        private static int
            _hashesId = Shader.PropertyToID("_Hashes"),
            _configId = Shader.PropertyToID("_Config");

        [SerializeField] private Mesh _instanceMesh;

        [SerializeField] private Material _material;

        [SerializeField, Range(1, 512)] private int _resolution = 16;

        private NativeArray<uint> _hashes;

        private ComputeBuffer _hashesBuffer;

        private MaterialPropertyBlock _propertyBlock;
        
        protected void OnEnable () 
        {
            int length = _resolution * _resolution;
            _hashes = new NativeArray<uint>(length, Allocator.Persistent);
            _hashesBuffer = new ComputeBuffer(length, 4);

            new HashJob {
                hashes = _hashes
            }.ScheduleParallel(_hashes.Length, _resolution, default).Complete();

            _hashesBuffer.SetData(_hashes);

            _propertyBlock ??= new MaterialPropertyBlock();
            _propertyBlock.SetBuffer(_hashesId, _hashesBuffer);
            _propertyBlock.SetVector(_configId, new Vector4(_resolution, 1f / _resolution));
        }
        
        protected void OnDisable () 
        {
            _hashes.Dispose();
            _hashesBuffer.Release();
            _hashesBuffer = null;
        }

        protected void OnValidate () 
        {
            if (_hashesBuffer != null && enabled) 
            {
                OnDisable();
                OnEnable();
            }
        }
        
        void Update () 
        {
            Graphics.DrawMeshInstancedProcedural(
                _instanceMesh, 0, _material, new Bounds(Vector3.zero, Vector3.one),
                _hashes.Length, _propertyBlock
            );
        }
    }
}