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
        public struct FaceSettings
        {
            const float k_ScaleMin = .00001f, k_ScaleMax = 10000f;

            public float scale { get; private set; }
            
            public float fade { get; private set; }
            
            public int faces { get; private set; }

            public FaceSettings(float scale, float fade, int faces)
            {
                this.fade = fade;
                this.scale = math.clamp(scale, k_ScaleMin, k_ScaleMax);
                this.faces = faces;
            }
        }
        
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
            where TVertexType : struct, SplineMesh.ISplineVertexData
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
        private static void ExtrudeSegments<T, K>(T spline, NativeArray<K> data,
            NativeArray<ExtrusionSegment> segments, ExtrusionSettings extrusionSettings, FaceSettings faceSettings, 
            int index, float length, int vertexArrayOffset)
            where T : ISpline
            where K : struct, SplineMesh.ISplineVertexData
        {
            float s = index / (extrusionSettings.segments - 1f);
            float t = math.lerp(extrusionSettings.range.x, extrusionSettings.range.y, s);
            float fade = faceSettings.fade == 0f ? 1f : math.smoothstep(0f, faceSettings.fade, math.abs(math.round(s) - s));
            float scale = fade * faceSettings.scale;
            ExtrudeInfo info = new ExtrudeInfo(t, length, scale, vertexArrayOffset + index * segments.Length * 2);
            ExtrudeSegments(spline, info, data, segments);
        }

        [BurstCompile]
        private static void ExtrudeSegments<T, K>(T spline, ExtrudeInfo info, NativeArray<K> data, NativeArray<ExtrusionSegment> segments)
            where T : ISpline
            where K : struct, SplineMesh.ISplineVertexData
        {
            float evaluationT = spline.Closed ? math.frac(info.T) : math.clamp(info.T, 0f, 1f);
            spline.Evaluate(evaluationT, out var sp, out var st, out var up);

            float tangentLength = math.lengthsq(st);
            if (tangentLength == 0f || float.IsNaN(tangentLength))
            {
                float adjustedT = math.clamp(evaluationT + (0.0001f * (info.T < 1f ? 1f : -1f)), 0f, 1f);
                spline.Evaluate(adjustedT, out _, out st, out up);
            }

            st = math.normalize(st);
            quaternion rot = quaternion.LookRotationSafe(st, up);
            int count = segments.Length;
            float y = info.T * info.Length;
            
            for (int n = 0; n < count; ++n)
            {
                ExtrusionSegment segment = segments[n];

                K MakeVertex(ExtrusionPoint point)
                {
                    K vertex = new K();
                
                    Vector3 position = point.position * info.Scale;
                    vertex.position = sp + math.rotate(rot, position);
                
                    Vector3 normal = point.normal;
                    vertex.normal = math.rotate(rot, normal);
                
                    vertex.texture = new Vector2(point.u, y);

                    return vertex;
                }

                int indexOffset = info.Start + 2 * n;

                data[indexOffset] = MakeVertex(segment.start);
                data[indexOffset + 1] = MakeVertex(segment.end);
            }
        }
        
        // switch to using IJobFor
        
        // note for simple shapes, parallel degrades performance
        
        [BurstCompile]
        struct ExtrudeFacesJob<K> : IJobParallelFor where K : struct, SplineMesh.ISplineVertexData
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
            
            // pre compute to avoid expensive GetLength on each iteration
            public float Length;

            public void Execute(int index)
            {
                ExtrudeSegments(Spline, Vertices, Segments, ExtrusionSettings, FaceSettings, index, Length, VertexArrayOffset);
            }
        }
        
        public static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, FaceSettings faceSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = faceSettings.faces * 2 * extrusionSettings.segments;
            indexCount = faceSettings.faces * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1));
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
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
           
            if (faceSettings.faces < 1)
                throw new ArgumentOutOfRangeException(nameof(faceSettings.faces), "Faces must be greater than 0");

            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 2");
            
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, faceSettings, vertexArrayOffset, indexArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, faceSettings, vertexArrayOffset, indexArrayOffset);
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
                    VertexArrayOffset = vertexArrayOffset,
                    Length = spline.GetLength()
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
                where TVertexType : struct, SplineMesh.ISplineVertexData
                where TIndexType : struct
        {
            if (faceSettings.faces < 1)
                throw new ArgumentOutOfRangeException(nameof(faceSettings.faces), "Faces must be greater than 0");

            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 1");
           
            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, faceSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, faceSettings, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            float length = spline.GetLength();
            for (int i = 0; i < extrusionSettings.segments; ++i)
            {
                ExtrudeSegments(spline, vertices, segments, extrusionSettings, faceSettings, i, length, vertexArrayOffset);           
            }
        }
        
        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt16> indices, ExtrusionSettings extrusionSettings, FaceSettings faceSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            bool closed = extrusionSettings.closed;
            int segments = extrusionSettings.segments;
            int faces = faceSettings.faces;
            int points = faces * 2;

            // loop over the segments along the length of the spline 
            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                // loop over the sides of a given spline extrusion segments
                for (int n = 0; n < faces; ++n)
                {
                    // takes two vertices of the current segment and the two corresponding vertices of the next segment
                    int index0 = vertexArrayOffset + i * points + n * 2;
                    int index1 = vertexArrayOffset + i * points + n * 2 + 1;
                    int index2 = vertexArrayOffset + ((i+1) % segments) * points + n * 2;
                    int index3 = vertexArrayOffset + ((i+1) % segments) * points + n * 2 + 1;

                    int indexOffset = i * faces * 6 + n * 6;
                    
                    // for a face we have two triangles, therefore we times the i sides and n faces 
                    indices[indexArrayOffset + indexOffset + 0] = (UInt16) index0;
                    indices[indexArrayOffset + indexOffset + 1] = (UInt16) index1;
                    indices[indexArrayOffset + indexOffset + 2] = (UInt16) index2;
                    indices[indexArrayOffset + indexOffset + 3] = (UInt16) index1;
                    indices[indexArrayOffset + indexOffset + 4] = (UInt16) index3;
                    indices[indexArrayOffset + indexOffset + 5] = (UInt16) index2;
                }
            }
        }

        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt32> indices, ExtrusionSettings extrusionSettings, FaceSettings faceSettings, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            bool closed = extrusionSettings.closed;
            int segments = extrusionSettings.segments;
            int faces = faceSettings.faces;
            int points = faces * 2;

            // loop over the segments along the length of the spline 
            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                // loop over the sides of a given spline extrusion segments
                for (int n = 0; n < faces; ++n)
                {
                    // takes two vertices of the current segment and the two corresponding vertices of the next segment
                    var index0 = vertexArrayOffset + i * points + n;
                    var index1 = vertexArrayOffset + i * points + ((n + 1) % points);
                    var index2 = vertexArrayOffset + ((i+1) % segments) * points + n;
                    var index3 = vertexArrayOffset + ((i+1) % segments) * points + ((n + 1) % points);

                    int indexOffset = i * faces * 6 + n * 6;
                    
                    // for a face we have two triangles, therefore we times the i sides and n faces 
                    indices[indexArrayOffset + indexOffset + 0] = (UInt16) index0;
                    indices[indexArrayOffset + indexOffset + 1] = (UInt16) index1;
                    indices[indexArrayOffset + indexOffset + 2] = (UInt16) index2;
                    indices[indexArrayOffset + indexOffset + 3] = (UInt16) index1;
                    indices[indexArrayOffset + indexOffset + 4] = (UInt16) index3;
                    indices[indexArrayOffset + indexOffset + 5] = (UInt16) index2;
                }
            }
        }
    }
}