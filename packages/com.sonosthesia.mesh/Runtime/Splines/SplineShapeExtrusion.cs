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
            where TVertexType : struct, IVertexData
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
        private static void ExtrudeShape<T, K>(T spline, NativeArray<K> data,
            NativeArray<ExtrusionPoint> shapePoints, ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, 
            int index, int vertexArrayOffset)
            where T : ISpline
            where K : struct, IVertexData
        {
            ExtrudeInfo info = new ExtrudeInfo(extrusionSettings, index, vertexArrayOffset + index * shapePoints.Length);
            ExtrudeShape(spline, info, data, shapePoints);
        }


        [BurstCompile]
        static void ExtrudeShape<T, K>(T spline, ExtrudeInfo info, NativeArray<K> data, NativeArray<ExtrusionPoint> shapePoints)
            where T : ISpline
            where K : struct, IVertexData
        {
            var evaluationT = info.closed ? math.frac(info.t) : math.clamp(info.t, 0f, 1f);
            spline.Evaluate(evaluationT, out var sp, out var st, out var up);

            var tangentLength = math.lengthsq(st);
            if (tangentLength == 0f || float.IsNaN(tangentLength))
            {
                var adjustedT = math.clamp(evaluationT + (0.0001f * (info.t < 1f ? 1f : -1f)), 0f, 1f);
                spline.Evaluate(adjustedT, out _, out st, out up);
            }

            st = math.normalize(st);

            var rot = quaternion.LookRotationSafe(st, up);
            int count = shapePoints.Length;

            for (int n = 0; n < count; ++n)
            {
                ExtrusionPoint point = shapePoints[n];
                
                var vertex = new K();
                
                Vector3 position = point.position * info.scale;
                vertex.position = sp + math.rotate(rot, position);
                
                Vector3 normal = point.normal;
                vertex.normal = math.rotate(rot, normal);
                
                vertex.texture = new Vector2(point.u, info.v);

                data[info.start + n] = vertex;
            }
        }
        
        // switch to using IJobFor
        
        // note for simple shapes, parallel degrades performance
        
        [BurstCompile]
        struct ExtrudeShapeJob<K> : IJobParallelFor where K : struct, IVertexData
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

            public void Execute(int index)
            {
                ExtrudeShape(Spline, Vertices, Points, ExtrusionSettings, ShapeSettings, index, VertexArrayOffset);
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
                where TVertexType : struct, IVertexData
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
                ShapeExtrusion.WindTris(ushortIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indexArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                ShapeExtrusion.WindTris(ulongIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indexArrayOffset);
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
                where TVertexType : struct, IVertexData
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
                ShapeExtrusion.WindTris(ushortIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                ShapeExtrusion.WindTris(ulongIndices, extrusionSettings, shapeSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            for (int i = 0; i < segments; ++i)
            {
                ExtrudeShape(spline, vertices, points, extrusionSettings, shapeSettings, i, vertexArrayOffset);
            }
        }
    }
}