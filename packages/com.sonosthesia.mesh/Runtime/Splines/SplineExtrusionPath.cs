using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    public class SplineExtrusionPath : ExtrusionPath
    {
        [SerializeField] private SplineReference _spline;

        [SerializeField] private bool _parallel;
        
        [SerializeField] private int _innerBatchCount = 100;
        
        protected virtual void OnEnable()
        {
            Spline.Changed += OnSplineChanged;
        }

        protected virtual void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }
        
        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (_spline?.Spline == spline)
            {
                BroadcastChange();
            }
        }
        
        public override float GetLength() => _spline?.Spline?.GetLength() ?? 0f;

        public override bool Populate(ref NativeArray<RigidTransform> points, float2 range, int segments)
        {
            points.TryReusePersistentArray(segments);
            
            Spline spline = _spline.Spline;

            if (_parallel && segments > _innerBatchCount * 2)
            {
                return PopulateParallel(spline, points, range, segments, spline.Closed);    
            }
            else
            {
                return PopulateSerial(spline, points, range, segments, spline.Closed);    
            }
        }

        [BurstCompile]
        private bool PopulateSerial(Spline spline , NativeArray<RigidTransform> points, float2 range, int segments, bool closed)
        {
            for (int index = 0; index < segments; ++index)
            {
                float s = index / (segments - 1f);
                float t = math.lerp(range.x, range.y, s);   
                
                var evaluationT = closed ? math.frac(t) : math.clamp(t, 0f, 1f);
                spline.Evaluate(evaluationT, out var sp, out var st, out var up);

                var tangentLength = math.lengthsq(st);
                if (tangentLength is 0f or Single.NaN)
                {
                    var adjustedT = math.clamp(evaluationT + (0.0001f * (t < 1f ? 1f : -1f)), 0f, 1f);
                    spline.Evaluate(adjustedT, out _, out st, out up);
                }

                st = math.normalize(st);

                var rot = quaternion.LookRotationSafe(st, up);

                points[index] = new RigidTransform(rot, sp);
            }

            return true;
        }
        
        [BurstCompile]
        private struct Job : IJobParallelFor 
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeSpline Spline;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<RigidTransform> Points;

            public float2 Range;
            public float Segments;
            public bool Closed;

            [BurstCompile]
            public void Execute(int index)
            {
                float s = index / (Segments - 1f);
                float t = math.lerp(Range.x, Range.y, s);   
                
                var evaluationT = Closed ? math.frac(t) : math.clamp(t, 0f, 1f);
                Spline.Evaluate(evaluationT, out var sp, out var st, out var up);

                var tangentLength = math.lengthsq(st);
                if (tangentLength is 0f or Single.NaN)
                {
                    var adjustedT = math.clamp(evaluationT + (0.0001f * (t < 1f ? 1f : -1f)), 0f, 1f);
                    Spline.Evaluate(adjustedT, out _, out st, out up);
                }

                st = math.normalize(st);

                var rot = quaternion.LookRotationSafe(st, up);

                Points[index] = new RigidTransform(rot, sp);
            }
        }
        
        private bool PopulateParallel(Spline spline , NativeArray<RigidTransform> points, float2 range, int segments, bool closed)
        {
            {
                using NativeSpline nativeSpline = new NativeSpline(spline, Allocator.TempJob);
                Job job = new Job()
                {
                    Spline = nativeSpline,
                    Points = points,
                    Range = range,
                    Segments = segments,
                    Closed = closed
                };
                job.Schedule(segments, (int)math.sqrt(segments)).Complete();
            }
            return true;
        }
    }
}