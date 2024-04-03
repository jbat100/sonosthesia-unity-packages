using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public class TorusKnotParametricPath : ParametricPath
    {
        [SerializeField] private float _scale;

        [SerializeField] private int _p;
        
        [SerializeField] private int _q;

        [SerializeField] private bool _SIMD;

        public override float GetLength() => 1f;
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job : IJob
        {
            public NativeArray<RigidTransform> points;

            public float2 range;

            public int segments;

            public float scale;

            public int p;

            public int q;

            public void Execute()
            {
                int steps = segments / 4;
                for (int offset = 0; offset < steps; ++offset)
                {
                    int4 indices = new int4(0, 1, 2, 3) + (offset * 4);

                    float4 s = (float4)indices / ((float4)segments - 1);
                    float4 t = math.lerp(range.x, range.y, s) * TAU;

                    math.sincos(q * t, out float4 sinQT, out float4 cosQT);
                    math.sincos(p * t, out float4 sinPT, out float4 cosPT);
                    
                    float4 r = cosQT + 2f;
                    float4 x = r * cosPT;
                    float4 y = r * sinPT;
                    float4 z = - sinQT;
                
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
        
        public override bool Populate(NativeArray<RigidTransform> points, float2 range, int segments)
        {
            if (_SIMD)
            {
                Job job = new Job
                {
                    points = points,
                    range = range,
                    segments = segments,
                    scale = _scale,
                    p = _p,
                    q = _q
                };
                job.Schedule().Complete();
            }
            else
            {
                for (int index = 0; index < segments; ++index)
                {
                    float s = index / (segments - 1f);
                    float t = math.lerp(range.x, range.y, s) * TAU;

                    float r = math.cos(_q * t) + 2f;
                    float x = r * math.cos(_p * t);
                    float y = r * math.sin(_p * t);
                    float z = - math.sin(_q * t);
                
                    float3 position = _scale * (new float3(x, y, z));

                    points[index] = new RigidTransform(quaternion.identity, position);
                }   
            }

            points.CalculateRotations(range);
            
            return true;
        }
    }
}