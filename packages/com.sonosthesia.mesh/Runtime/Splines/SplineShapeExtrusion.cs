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
            
            public int points { get; private set; }

            public bool closed { get; private set; }

            public int Sides => Mathf.Max(closed ? points : points - 1, 0);

            public ShapeSettings(float scale, int points, bool closed)
            {
                this.scale = math.clamp(scale, k_ScaleMin, k_ScaleMax);
                this.points = points;
                this.closed = closed;
            }
        }
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            bool parallel,
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionShapePoint> points,
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
        
        
        [BurstCompile]
        static void ExtrudeShape<T, K>(T spline, float t, NativeArray<K> data, NativeArray<ExtrusionShapePoint> shapePoints, ShapeSettings shapeSettings, int start)
            where T : ISpline
            where K : struct, SplineMesh.ISplineVertexData
        {
            var evaluationT = spline.Closed ? math.frac(t) : math.clamp(t, 0f, 1f);
            spline.Evaluate(evaluationT, out var sp, out var st, out var up);

            var tangentLength = math.lengthsq(st);
            if (tangentLength == 0f || float.IsNaN(tangentLength))
            {
                var adjustedT = math.clamp(evaluationT + (0.0001f * (t < 1f ? 1f : -1f)), 0f, 1f);
                spline.Evaluate(adjustedT, out _, out st, out up);
            }

            st = math.normalize(st);

            var rot = quaternion.LookRotationSafe(st, up);
            int count = shapePoints.Length;

            float splineLength = spline.GetLength();
            
            for (int n = 0; n < count; ++n)
            {
                ExtrusionShapePoint point = shapePoints[n];
                
                var vertex = new K();
                
                Vector3 position = point.position * shapeSettings.scale;
                vertex.position = sp + math.rotate(rot, position);
                
                Vector3 normal = point.normal;
                vertex.normal = math.rotate(rot, normal);
                
                vertex.texture = new Vector2(point.u, t * splineLength);

                data[start + n] = vertex;
            }
        }
        
        // switch to using IJobFor
        
        [BurstCompile]
        struct ExtrudeShapeJob<K> : IJobParallelFor where K : struct, SplineMesh.ISplineVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeSpline Spline;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<ExtrusionShapePoint> Points;
            
            public ExtrusionSettings ExtrusionSettings;
            public ShapeSettings ShapeSettings;
            public int VertexArrayOffset;

            public void Execute(int index)
            {
                float t = math.lerp(ExtrusionSettings.range.x, ExtrusionSettings.range.y, index / (ExtrusionSettings.segments - 1f));
                ExtrudeShape(Spline, t, Vertices, Points, ShapeSettings, VertexArrayOffset + index * Points.Length);
            }
        }
        
        public static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = shapeSettings.points * (extrusionSettings.segments + (extrusionSettings.capped ? 2 : 0));
            indexCount = shapeSettings.Sides * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1)) + (extrusionSettings.capped ? (shapeSettings.Sides - 2) * 6 : 0);
        }

        public static void ExtrudeParallel<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionShapePoint> points,
                ExtrusionSettings extrusionSettings,
                ShapeSettings shapeSettings,
                int vertexArrayOffset = 0,
                int indexArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
            int segments = extrusionSettings.segments;
            float2 range = extrusionSettings.range;
            bool capped = extrusionSettings.capped;

            GetVertexAndIndexCount(extrusionSettings, shapeSettings, out var vertexCount, out var indexCount);

            if (shapeSettings.Sides < 1)
                throw new ArgumentOutOfRangeException(nameof(shapeSettings.Sides), "Sides must be greater than 0");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 2");
            
            if (vertices.Length < vertexCount)
                throw new ArgumentOutOfRangeException($"Vertex array is incorrect size. Expected {vertexCount} or more, but received {vertices.Length}.");

            if (indices.Length < indexCount)
                throw new ArgumentOutOfRangeException($"Index array is incorrect size. Expected {indexCount} or more, but received {indices.Length}.");

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
                    VertexArrayOffset = vertexArrayOffset
                };
                job.Schedule(segments, (int)math.sqrt(segments)).Complete();
            }
            
            if (capped)
            {
                int capVertexStart = vertexArrayOffset + segments * shapeSettings.points;
                int endCapVertexStart = vertexArrayOffset + (segments + 1) * shapeSettings.points;

                float2 rng = spline.Closed ? math.frac(range) : math.clamp(range, 0f, 1f);
                ExtrudeShape(spline, rng.x, vertices, points, shapeSettings, capVertexStart);
                ExtrudeShape(spline, rng.y, vertices, points, shapeSettings, endCapVertexStart);
            }
        }
        
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionShapePoint> points,
                ExtrusionSettings extrusionSettings,
                ShapeSettings shapeSettings,
                int vertexArrayOffset = 0,
                int indicesArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
            
            var segments = extrusionSettings.segments;
            var range = extrusionSettings.range;
            var capped = extrusionSettings.capped;

            GetVertexAndIndexCount(extrusionSettings, shapeSettings, out var vertexCount, out var indexCount);

            if (shapeSettings.Sides < 1)
                throw new ArgumentOutOfRangeException(nameof(shapeSettings.Sides), "Sides must be greater than 0");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 1");
            
            if (vertices.Length < vertexCount)
                throw new ArgumentOutOfRangeException($"Vertex array is incorrect size. Expected {vertexCount} or more, but received {vertices.Length}.");

            if (indices.Length < indexCount)
                throw new ArgumentOutOfRangeException($"Index array is incorrect size. Expected {indexCount} or more, but received {indices.Length}.");

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

            for (int i = 0; i < segments; ++i)
                ExtrudeShape(spline, math.lerp(range.x, range.y, i / (segments - 1f)), vertices, points, shapeSettings, vertexArrayOffset + i * shapeSettings.points);

            if (capped)
            {
                int capVertexStart = vertexArrayOffset + segments * shapeSettings.points;
                int endCapVertexStart = vertexArrayOffset + (segments + 1) * shapeSettings.points;

                float2 rng = spline.Closed ? math.frac(range) : math.clamp(range, 0f, 1f);
                ExtrudeShape(spline, rng.x, vertices, points, shapeSettings, capVertexStart);
                ExtrudeShape(spline, rng.y, vertices, points, shapeSettings, endCapVertexStart);
            }
        }
        
        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        // Note this winds the tris of the whole extruded spline mesh and should work with any extruded shape
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt16> indices, ExtrusionSettings settings, ShapeSettings shapeSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            bool closed = settings.closed;
            int segments = settings.segments;
            bool capped = settings.capped;

            int shapePoints = shapeSettings.points;
            int shapeSides = shapeSettings.Sides;

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

            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * shapePoints;
                var capIndexStart = indexArrayOffset + shapePoints * 6 * (segments-1);
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * shapePoints;
                var endCapIndexStart = indexArrayOffset + (segments-1) * 6 * shapePoints + (shapePoints-2) * 3;

                for(ushort i = 0; i < shapeSides - 2; ++i)
                {
                    indices[capIndexStart + i * 3 + 0] = (UInt16)(capVertexStart);
                    indices[capIndexStart + i * 3 + 1] = (UInt16)(capVertexStart + i + 2);
                    indices[capIndexStart + i * 3 + 2] = (UInt16)(capVertexStart + i + 1);

                    indices[endCapIndexStart + i * 3 + 0] = (UInt16) (endCapVertexStart);
                    indices[endCapIndexStart + i * 3 + 1] = (UInt16) (endCapVertexStart + i + 1);
                    indices[endCapIndexStart + i * 3 + 2] = (UInt16) (endCapVertexStart + i + 2);
                }
            }
        }

        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt32> indices, ExtrusionSettings settings, ShapeSettings shapeSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;
            var capped = settings.capped;
            
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

            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * shapePoints;
                var capIndexStart = indexArrayOffset + shapePoints * 6 * (segments-1);
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * shapePoints;
                var endCapIndexStart = indexArrayOffset + (segments-1) * 6 * shapePoints + (shapePoints-2) * 3;

                for(ushort i = 0; i < shapeSides - 2; ++i)
                {
                    indices[capIndexStart + i * 3 + 0] = (UInt32)(capVertexStart);
                    indices[capIndexStart + i * 3 + 1] = (UInt32)(capVertexStart + i + 2);
                    indices[capIndexStart + i * 3 + 2] = (UInt32)(capVertexStart + i + 1);

                    indices[endCapIndexStart + i * 3 + 0] = (UInt32) (endCapVertexStart);
                    indices[endCapIndexStart + i * 3 + 1] = (UInt32) (endCapVertexStart + i + 1);
                    indices[endCapIndexStart + i * 3 + 2] = (UInt32) (endCapVertexStart + i + 2);
                }
            }
        }
    }
}