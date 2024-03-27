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
    public static class SplineShapeExtrusion
    {
        public struct ShapeSettings
        {
            const float k_ScaleMin = .00001f, k_ScaleMax = 10000f;

            public float scale { get; private set; }
            
            public float fade { get; private set; }
            
            public int points { get; private set; }

            public bool closed { get; private set; }

            public int Sides => Mathf.Max(closed ? points : points - 1, 0);

            public ShapeSettings(float scale, float fade, int points, bool closed)
            {
                this.scale = math.clamp(scale, k_ScaleMin, k_ScaleMax);
                this.fade = fade;
                this.points = points;
                this.closed = closed;
            }
        }
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            bool parallel,
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionPoint> points,
            ExtrusionSettings extrusionSettings,
            ShapeSettings shapeSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TSplineType : ISpline
            where TVertexType : struct, SplineMesh.ISplineVertexData
            where TIndexType : struct
        {
            if (parallel)
            {
                ExtrudeParallel(spline, vertices, indices, points,
                    extrusionSettings, shapeSettings,
                    vertexArrayOffset, indicesArrayOffset);  
            }
            else
            {
                Extrude(spline, vertices, indices, points,
                    extrusionSettings, shapeSettings,
                    vertexArrayOffset, indicesArrayOffset);    
            }
        }
        
        private readonly struct ExtrudeInfo
        {
            public readonly float T;
            public readonly float Length;
            public readonly float Scale;
            public readonly int Start;

            public ExtrudeInfo(float t, float length, float scale, int start)
            {
                T = t;
                Length = length;
                Scale = scale;
                Start = start;
            }
        }

        [BurstCompile]
        private static void ExtrudeShape<T, K>(T spline, NativeArray<K> data,
            NativeArray<ExtrusionPoint> shapePoints, ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, 
            int index, float length, int vertexArrayOffset)
            where T : ISpline
            where K : struct, SplineMesh.ISplineVertexData
        {
            float s = index / (extrusionSettings.segments - 1f);
            float t = math.lerp(extrusionSettings.range.x, extrusionSettings.range.y, s);
            float fade = shapeSettings.fade == 0f ? 1f : math.smoothstep(0f, shapeSettings.fade, math.abs(math.round(s) - s));
            float scale = fade * shapeSettings.scale;
            ExtrudeInfo info = new ExtrudeInfo(t, length, scale, vertexArrayOffset + index * shapePoints.Length);
            ExtrudeShape(spline, info, data, shapePoints);
        }


        [BurstCompile]
        static void ExtrudeShape<T, K>(T spline, ExtrudeInfo info, NativeArray<K> data, NativeArray<ExtrusionPoint> shapePoints)
            where T : ISpline
            where K : struct, SplineMesh.ISplineVertexData
        {
            var evaluationT = spline.Closed ? math.frac(info.T) : math.clamp(info.T, 0f, 1f);
            spline.Evaluate(evaluationT, out var sp, out var st, out var up);

            var tangentLength = math.lengthsq(st);
            if (tangentLength == 0f || float.IsNaN(tangentLength))
            {
                var adjustedT = math.clamp(evaluationT + (0.0001f * (info.T < 1f ? 1f : -1f)), 0f, 1f);
                spline.Evaluate(adjustedT, out _, out st, out up);
            }

            st = math.normalize(st);

            var rot = quaternion.LookRotationSafe(st, up);
            int count = shapePoints.Length;

            for (int n = 0; n < count; ++n)
            {
                ExtrusionPoint point = shapePoints[n];
                
                var vertex = new K();
                
                Vector3 position = point.position * info.Scale;
                vertex.position = sp + math.rotate(rot, position);
                
                Vector3 normal = point.normal;
                vertex.normal = math.rotate(rot, normal);
                
                vertex.texture = new Vector2(point.u, info.T);

                data[info.Start + n] = vertex;
            }
        }
        
        // switch to using IJobFor
        
        // note for simple shapes, parallel degrades performance
        
        [BurstCompile]
        struct ExtrudeShapeJob<K> : IJobParallelFor where K : struct, SplineMesh.ISplineVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeSpline Spline;
            
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<ExtrusionPoint> Points;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;

            public ExtrusionSettings ExtrusionSettings;
            public ShapeSettings ShapeSettings;
            public int VertexArrayOffset;
            
            // pre compute to avoid expensive GetLength on each iteration
            public float Length;

            public void Execute(int index)
            {
                ExtrudeShape(Spline, Vertices, Points, ExtrusionSettings, ShapeSettings, index, Length, VertexArrayOffset);
            }
        }
        
        public static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = shapeSettings.points * extrusionSettings.segments;
            indexCount = shapeSettings.Sides * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1));
        }

        public static void ExtrudeParallel<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionPoint> points,
                ExtrusionSettings extrusionSettings,
                ShapeSettings shapeSettings,
                int vertexArrayOffset = 0,
                int indexArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
            int segments = extrusionSettings.segments;
            
            if (shapeSettings.Sides < 1)
                throw new ArgumentOutOfRangeException(nameof(shapeSettings.Sides), "Sides must be greater than 0");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 2");
            
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indexArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indexArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            {
                using NativeSpline nativeSpline = new NativeSpline(spline, Allocator.TempJob);
                ExtrudeShapeJob<TVertexType> job = new ExtrudeShapeJob<TVertexType>()
                {
                    Spline = nativeSpline,
                    Vertices = vertices,
                    Points = points,
                    ExtrusionSettings = extrusionSettings,
                    ShapeSettings = shapeSettings,
                    VertexArrayOffset = vertexArrayOffset,
                    Length = spline.GetLength()
                };
                job.Schedule(segments, (int)math.sqrt(segments)).Complete();
            }
        }

        public static void Extrude<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionPoint> points,
                ExtrusionSettings extrusionSettings,
                ShapeSettings shapeSettings,
                int vertexArrayOffset = 0,
                int indicesArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
            
            int segments = extrusionSettings.segments;
            
            if (shapeSettings.Sides < 1)
                throw new ArgumentOutOfRangeException(nameof(shapeSettings.Sides), "Sides must be greater than 0");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 1");
            
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            float length = spline.GetLength();
            
            for (int i = 0; i < segments; ++i)
            {
                ExtrudeShape(spline, vertices, points, extrusionSettings, shapeSettings, i, length, vertexArrayOffset);
            }
            
        }
        
        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        // Note this winds the tris of the whole extruded spline mesh and should work with any extruded shape
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt16> indices, ExtrusionSettings settings, ShapeSettings shapeSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            bool closed = settings.closed;
            int segments = settings.segments;

            int shapePoints = shapeSettings.points;
            int shapeSides = shapeSettings.Sides;
            
            Debug.Log($"{nameof(SplineShapeExtrusion)} {nameof(WindTris)} for {segments} segments");

            // loop over the segments along the length of the spline 
            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                // loop over the sides of a given spline extrusion segments
                for (int n = 0; n < shapeSides; ++n)
                {
                    // takes two vertices of the current segment and the two corresponding vertices of the next segment
                    var index0 = vertexArrayOffset + i * shapePoints + n;
                    var index1 = vertexArrayOffset + i * shapePoints + ((n + 1) % shapePoints);
                    var index2 = vertexArrayOffset + ((i+1) % segments) * shapePoints + n;
                    var index3 = vertexArrayOffset + ((i+1) % segments) * shapePoints + ((n + 1) % shapePoints);

                    // for a face we have two triangles, therefore we times the i sides and n faces 
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 0] = (UInt16) index0;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 1] = (UInt16) index1;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 2] = (UInt16) index2;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 3] = (UInt16) index1;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 4] = (UInt16) index3;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 5] = (UInt16) index2;
                }
            }
        }

        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt32> indices, ExtrusionSettings settings, ShapeSettings shapeSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;
            
            int shapePoints = shapeSettings.points;
            int shapeSides = shapeSettings.Sides;

            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                for (int n = 0; n < shapeSides; ++n)
                {
                    var index0 = vertexArrayOffset + i * shapePoints + n;
                    var index1 = vertexArrayOffset + i * shapePoints + ((n + 1) % shapePoints);
                    var index2 = vertexArrayOffset + ((i+1) % segments) * shapePoints + n;
                    var index3 = vertexArrayOffset + ((i+1) % segments) * shapePoints + ((n + 1) % shapePoints);

                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 0] = (UInt32) index0;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 1] = (UInt32) index1;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 2] = (UInt32) index2;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 3] = (UInt32) index1;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 4] = (UInt32) index3;
                    indices[indexArrayOffset + i * shapeSides * 6 + n * 6 + 5] = (UInt32) index2;
                }
            }
        }
    }
}