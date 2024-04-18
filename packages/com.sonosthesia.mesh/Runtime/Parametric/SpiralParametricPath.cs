using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public class SpiralParametricPath : ParametricPath
    {
        [SerializeField] private AnimationCurve _radiusCurve;
        
        [SerializeField] private AnimationCurve _heightCurve;

        [SerializeField] private float _scale = 1f;
        
        [SerializeField] private float _revolutions = 2;

        [SerializeField] private float _phase;

        [SerializeField] private bool _SIMD;

        private bool _dirty;
        private NativeArray<float> _radii;
        private NativeArray<float> _heights;
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job : IJob
        {
            public NativeArray<RigidTransform> points;

            public NativeArray<float> radii;
            
            public NativeArray<float> heights;

            public int segments;

            public float2 range;

            public float phase;

            public float revolutions;

            public float scale;

            public void Execute()
            {
                float tauRevolutions = TAU * revolutions;
                int steps = segments / 4;
                for (int step = 0; step < steps; ++step)
                {
                    int offset = step * 4;
                    int4 indices = new int4(0, 1, 2, 3) + offset;

                    float4 s = (float4)indices / ((float4)segments - 1);
                    float4 t = phase + math.lerp(range.x, range.y, s) * tauRevolutions;

                    math.sincos(t, out float4 sinT, out float4 cosT);

                    // TODO : use Reinterpret outside loop
                    
                    float4 r = radii.Slice4(offset);
                    float4 x = r * cosT;
                    float4 y = heights.Slice4(offset);
                    float4 z = r * sinT;
                
                    float4x3 positions = new float4x3(x, y, z) * scale;
                    float3x4 transposed = math.transpose(positions);

                    for (int i = 0; i < 4; ++i)
                    {
                        int index = indices[i];
                        if (index < segments)
                        {
                            points[index] = new RigidTransform(quaternion.identity, transposed[i]);    
                        }    
                    }
                }
            }
        }

        protected override void OnChanged() => _dirty = true;

        protected virtual void OnDisable()
        {
            _radii.Dispose();
            _heights.Dispose();
        }

        public override float GetLength() => 1f;
        
        public override bool Populate(NativeArray<RigidTransform> points, float2 range, int segments)
        {
            if (_SIMD)
            {
                _radii.TryReusePersistentArray(segments);
                _heights.TryReusePersistentArray(segments);
            
                if (_dirty)
                {
                    _radiusCurve.Populate(_radii);
                    _heightCurve.Populate(_heights);
                    _dirty = false;
                }

                Job job = new Job
                {
                    points = points,
                    range = range,
                    segments = segments,
                    scale = _scale,
                    revolutions = _revolutions,
                    phase = _phase,
                    radii = _radii,
                    heights = _heights
                };
                job.Run();                
            }
            else
            {
                for (int index = 0; index < segments; ++index)
                {
                    float s = math.lerp(range.x, range.y, index / ((float)segments - 1));
                    float t = _phase + s * TAU * _revolutions;

                    math.sincos(t, out float sinT, out float cosT);

                    float r = _radiusCurve.Evaluate(s);
                    float x = r * cosT;
                    float y = _heightCurve.Evaluate(s);
                    float z = r * sinT;
                
                    points[index] = new RigidTransform(quaternion.identity, new float3(x, y, z) * _scale);  
                }
            }
            
            return true;
        }
    }
}