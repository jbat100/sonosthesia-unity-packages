using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Fractal : MonoBehaviour    
    {
        private static MaterialPropertyBlock _propertyBlock;
        
        private static readonly int _matricesId = Shader.PropertyToID("_Matrices");
        
        [SerializeField, Range(1, 8)] private int _depth = 4;
        
        [SerializeField] private Mesh _mesh;

        [SerializeField] private Material _material;

        private static readonly Vector3[] _directions = 
        {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
        };

        static readonly Quaternion[] _rotations = 
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
        };
        
        struct FractalPart 
        {
            public Vector3 direction, worldPosition;
            public Quaternion rotation, worldRotation;
            public float spinAngle;
        }
        
        private FractalPart[][] _parts;
        private Matrix4x4[][] _matrices;
        private ComputeBuffer[] _matricesBuffers;
        
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
            
            _parts = new FractalPart[_depth][];
            _matrices = new Matrix4x4[_depth][];
            _matricesBuffers = new ComputeBuffer[_depth];
            int stride = 16 * 4;
            for (int i = 0, length = 1; i < _parts.Length; i++, length *= 5) 
            {
                _parts[i] = new FractalPart[length];
                _matrices[i] = new Matrix4x4[length];
                _matricesBuffers[i] = new ComputeBuffer(length, stride);
            }
            _parts[0][0] = CreatePart(0);
            
            for (int li = 1; li < _parts.Length; li++) 
            {
                FractalPart[] levelParts = _parts[li];
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
            }
            _parts = null;
            _matrices = null;
            _matricesBuffers = null;
        }
        
        protected void Update () 
        {
            float spinAngleDelta = 22.5f * Time.deltaTime;
            
            FractalPart rootPart = _parts[0][0];
            rootPart.spinAngle += spinAngleDelta;
            rootPart.worldRotation = rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f);
            _parts[0][0] = rootPart;
            _matrices[0][0] = Matrix4x4.TRS(
                rootPart.worldPosition, rootPart.worldRotation, Vector3.one
            );
            
            float scale = 1f;
            for (int li = 1; li < _parts.Length; li++) 
            {
                scale *= 0.5f;
                FractalPart[] parentParts = _parts[li - 1];
                FractalPart[] levelParts = _parts[li];
                Matrix4x4[] levelMatrices = _matrices[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi++) 
                {
                    FractalPart parent = parentParts[fpi / 5];
                    FractalPart part = levelParts[fpi];
                    part.spinAngle += spinAngleDelta;
                    part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));;
                    part.worldPosition =
                        parent.worldPosition +
                        parent.worldRotation * (1.5f  * scale * part.direction);
                    levelParts[fpi] = part;
                    levelMatrices[fpi] = Matrix4x4.TRS(
                        part.worldPosition, part.worldRotation, scale * Vector3.one
                    );
                }
            }

            var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
            for (int i = 0; i < _matricesBuffers.Length; i++) 
            {
                ComputeBuffer buffer = _matricesBuffers[i];
                buffer.SetData(_matrices[i]);   
                _propertyBlock.SetBuffer(_matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, buffer.count, _propertyBlock);
            }
        }
        
        private FractalPart CreatePart (int childIndex) 
        {
            return new FractalPart() {
                direction = _directions[childIndex],
                rotation = _rotations[childIndex]
            };
        }
    }
}