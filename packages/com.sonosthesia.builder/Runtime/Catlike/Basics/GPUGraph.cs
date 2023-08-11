using UnityEngine;

namespace Sonosthesia.Builder
{
    public class GPUGraph : MonoBehaviour
    {
        public enum FunctionName
        {
            Wave, 
            MultiWave, 
            Ripple,
            Sphere,
            Torus
        }
        
        private static readonly int
            _positionsId = Shader.PropertyToID("_Positions"),
            _resolutionId = Shader.PropertyToID("_Resolution"),
            _stepId = Shader.PropertyToID("_Step"),
            _timeId = Shader.PropertyToID("_Time");
        
        [SerializeField] private Material _material;

        [SerializeField] private Mesh _mesh;
        
        [SerializeField, Range(10, 1000)] private int _resolution = 10;

        [SerializeField] private ComputeShader _computeShader;

        [SerializeField] private FunctionName _functionName;
        
        private ComputeBuffer _positionsBuffer;

        protected void OnEnable () 
        {
            _positionsBuffer = new ComputeBuffer(_resolution * _resolution, 3 * 4);
        }

        protected void OnDisable()
        {
            _positionsBuffer.Release();
            _positionsBuffer = null;
        }
        
        protected void Update()
        {
            UpdateFunctionOnGPU();
        }
        
        private void UpdateFunctionOnGPU () 
        {
            int kernelIndex = (int)_functionName;
            
            float step = 2f / _resolution;
            _computeShader.SetInt(_resolutionId, _resolution);
            _computeShader.SetFloat(_stepId, step);
            _computeShader.SetFloat(_timeId, Time.time);
            _computeShader.SetBuffer(kernelIndex, _positionsId, _positionsBuffer);
            int groups = Mathf.CeilToInt(_resolution / 8f);
            _computeShader.Dispatch(kernelIndex, groups, groups, 1);
            _material.SetBuffer(_positionsId, _positionsBuffer);
            _material.SetFloat(_stepId, step);
            var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / _resolution));
            Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, _positionsBuffer.count);
        }
    }
}