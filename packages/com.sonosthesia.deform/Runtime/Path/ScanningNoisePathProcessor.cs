using Sonosthesia.Mesh;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class ScanningNoisePathProcessor : PathProcessor
    {
        [SerializeField] private float _displacement = 0.1f;
        
        [SerializeField] private float _scale = 1f;

        [SerializeField] private Vector3 _offset;

        [SerializeField] private Vector3 _direction = Vector3.up;

        [SerializeField] private float _speed = 1f;

        private float _time;

        protected override void OnEnable()
        {
            base.OnEnable();
            _time = 0f;
        }

        protected virtual void Update()
        {
            _time += Time.deltaTime * _speed;
        }

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        private struct Job : IJob
        {
            public NativeArray<RigidTransform> points;
            public float scale;
            public float displacement;
            public float3 offset;
            public float3 direction;
            public float time;

            public void Execute()
            {
                int length = points.Length;
                for (int index = 0; index < length; ++index)
                {
                    RigidTransform point = points[index];
                    float noise = Unity.Mathematics.noise.snoise(new float4(point.pos * scale + offset, time));
                    point.pos += math.mul(point.rot, direction) * displacement * noise;
                    points[index] = point;
                }
            }
        }
        
        public override void Process(NativeArray<RigidTransform> points)
        {
            Job job = new Job
            {
                points = points,
                scale = _scale,
                offset = _offset,
                direction = _direction,
                displacement = _displacement,
                time = _time
            };
            job.Schedule().Complete();
        }
    }
}