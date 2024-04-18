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
    public static class SplineFaceExtrusion
    {
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            bool parallel,
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionSegment> segments,
            ExtrusionSettings extrusionSettings,
            FaceSettings faceSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TSplineType : ISpline
            where TVertexType : struct, IVertexData
            where TIndexType : struct
        {
            if (parallel)
            {
                ExtrudeParallel(spline, vertices, indices, segments,
                    extrusionSettings, faceSettings,
                    vertexArrayOffset, indicesArrayOffset);  
            }
            else
            {
                Extrude(spline, vertices, indices, segments,
                    extrusionSettings, faceSettings,
                    vertexArrayOffset, indicesArrayOffset);    
            }
        }

        [BurstCompile]
        private static void ExtrudeSegments<T, K>(T spline, NativeArray<K> data,
            NativeArray<ExtrusionSegment> segments, ExtrusionSettings extrusionSettings, 
            int index, int vertexArrayOffset)
            where T : ISpline
            where K : struct, IVertexData
        {
            ExtrudeInfo info = new ExtrudeInfo(extrusionSettings, index, vertexArrayOffset + index * segments.Length * 2);
            ExtrudeSegments(spline, info, data, segments);
        }

        [BurstCompile]
        private static void ExtrudeSegments<T, K>(T spline, ExtrudeInfo info, NativeArray<K> data, NativeArray<ExtrusionSegment> segments)
            where T : ISpline
            where K : struct, IVertexData
        {
            float evaluationT = info.closed ? math.frac(info.t) : math.clamp(info.t, 0f, 1f);
            spline.Evaluate(evaluationT, out var sp, out var st, out var up);

            float tangentLength = math.lengthsq(st);
            if (tangentLength == 0f || float.IsNaN(tangentLength))
            {
                float adjustedT = math.clamp(evaluationT + (0.0001f * (info.t < 1f ? 1f : -1f)), 0f, 1f);
                spline.Evaluate(adjustedT, out _, out st, out up);
            }

            st = math.normalize(st);
            quaternion rot = quaternion.LookRotationSafe(st, up);
            int count = segments.Length;
            
            for (int n = 0; n < count; ++n)
            {
                ExtrusionSegment segment = segments[n];

                K MakeVertex(ExtrusionPoint point)
                {
                    K vertex = new K();
                
                    Vector3 position = point.position * info.scale;
                    vertex.position = sp + math.rotate(rot, position);
                
                    Vector3 normal = point.normal;
                    vertex.normal = math.rotate(rot, normal);
                
                    vertex.texture = new Vector2(point.u, info.v);

                    return vertex;
                }

                int indexOffset = info.start + 2 * n;

                data[indexOffset] = MakeVertex(segment.start);
                data[indexOffset + 1] = MakeVertex(segment.end);
            }
        }
        
        // switch to using IJobFor
        
        // note for simple shapes, parallel degrades performance
        
        [BurstCompile]
        struct ExtrudeFacesJob<K> : IJobParallelFor where K : struct, IVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeSpline Spline;
            
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<ExtrusionSegment> Segments;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;

            public ExtrusionSettings ExtrusionSettings;
            public FaceSettings FaceSettings;
            public int VertexArrayOffset;

            public void Execute(int index)
            {
                ExtrudeSegments(Spline, Vertices, Segments, ExtrusionSettings, index, VertexArrayOffset);
            }
        }

        public static void ExtrudeParallel<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionSegment> segments,
                ExtrusionSettings extrusionSettings,
                FaceSettings faceSettings,
                int vertexArrayOffset = 0,
                int indexArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, IVertexData
                where TIndexType : struct
        {
           
            if (faceSettings.faces < 1)
                throw new ArgumentOutOfRangeException(nameof(faceSettings.faces), "Faces must be greater than 0");

            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 2");
            
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                FaceExtrusion.WindTris(ushortIndices, extrusionSettings, faceSettings, vertexArrayOffset, indexArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                FaceExtrusion.WindTris(ulongIndices, extrusionSettings, faceSettings, vertexArrayOffset, indexArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            {
                using NativeSpline nativeSpline = new NativeSpline(spline, Allocator.TempJob);
                ExtrudeFacesJob<TVertexType> job = new ExtrudeFacesJob<TVertexType>()
                {
                    Spline = nativeSpline,
                    Vertices = vertices,
                    Segments = segments,
                    ExtrusionSettings = extrusionSettings,
                    FaceSettings = faceSettings,
                    VertexArrayOffset = vertexArrayOffset
                };
                job.Schedule(extrusionSettings.segments, (int)math.sqrt(extrusionSettings.segments)).Complete();
            }
        }

        public static void Extrude<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionSegment> segments,
                ExtrusionSettings extrusionSettings,
                FaceSettings faceSettings,
                int vertexArrayOffset = 0,
                int indicesArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, IVertexData
                where TIndexType : struct
        {
            if (faceSettings.faces < 1)
                throw new ArgumentOutOfRangeException(nameof(faceSettings.faces), "Faces must be greater than 0");

            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 1");
           
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                FaceExtrusion.WindTris(ushortIndices, extrusionSettings, faceSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                FaceExtrusion.WindTris(ulongIndices, extrusionSettings, faceSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            for (int i = 0; i < extrusionSettings.segments; ++i)
            {
                ExtrudeSegments(spline, vertices, segments, extrusionSettings, i, vertexArrayOffset);           
            }
        }
        
    }
}