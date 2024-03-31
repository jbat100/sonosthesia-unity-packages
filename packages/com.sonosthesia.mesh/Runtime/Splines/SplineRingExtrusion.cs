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
    public static class SplineRingExtrusion
    {
        public readonly struct RingSettings
        {
            const int k_SidesMin = 3, k_SidesMax = 2084;

            public readonly int sides;

            public readonly bool capped;

            public RingSettings(int sides, bool capped)
            {
                this.capped = capped;
                this.sides = math.clamp(sides, k_SidesMin, k_SidesMax);
            }
        }
        
        private static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, RingSettings ringSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = ringSettings.sides * (extrusionSettings.segments + (ringSettings.capped ? 2 : 0));
            indexCount = ringSettings.sides * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1)) + (ringSettings.capped ? (ringSettings.sides - 2) * 3 * 2 : 0);
        }

        /// <summary>
        /// Interface for Spline mesh vertex data. Implement this interface if you are extruding custom mesh data and
        /// do not want to use the vertex layout provided by <see cref="SplineMesh"/>."/>.
        /// </summary>
        
        [BurstCompile]
        static void ExtrudeRing<T, K>(T spline, NativeArray<K> data, ExtrudeInfo info, RingSettings ringSettings)
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

            quaternion rot = quaternion.LookRotationSafe(st, up);
            float rad = math.radians(360f / ringSettings.sides);
            
            for (int n = 0; n < ringSettings.sides; ++n)
            {
                var vertex = new K();
                var p = new float3(math.cos(n * rad), math.sin(n * rad), 0f) * info.scale;
                vertex.position = sp + math.rotate(rot, p);
                vertex.normal = (vertex.position - (Vector3)sp).normalized;

                // instead of inserting a seam, wrap UVs using a triangle wave so that texture wraps back onto itself
                float ut = n / ((float)ringSettings.sides + ringSettings.sides%2);
                float u = math.abs(ut - math.floor(ut + .5f)) * 2f;
                vertex.texture = new Vector2(u, info.v);

                data[info.start + n] = vertex;
            }
        }
        
        public static void Extrude<TSplineType, TVertexType, TIndexType>(
            bool parallel,
            TSplineType spline,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            RingSettings ringSettings,
            ExtrusionSettings extrusionSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TSplineType : ISpline
            where TVertexType : struct, IVertexData
            where TIndexType : struct
        {
            if (parallel)
            {
                ExtrudeParallel(spline, vertices, indices, extrusionSettings, ringSettings, vertexArrayOffset, indicesArrayOffset);  
            }
            else
            {
                Extrude(spline, vertices, indices, extrusionSettings, ringSettings, vertexArrayOffset, indicesArrayOffset);    
            }
        }

        // switch to using IJobFor
        
        [BurstCompile]
        struct ExtrudeRingJob<K> : IJobParallelFor where K : struct, IVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeSpline Spline;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;
            
            public ExtrusionSettings ExtrusionSettings;
            public RingSettings RingSettings;
            public int VertexArrayOffset;

            public void Execute(int index)
            {
                ExtrudeInfo info = new ExtrudeInfo(ExtrusionSettings, index, VertexArrayOffset + index * RingSettings.sides);
                ExtrudeRing(Spline, Vertices, info, RingSettings);
            }
        }

        static void ExtrudeParallel<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                ExtrusionSettings extrusionSettings,
                RingSettings ringSettings,
                int vertexArrayOffset = 0,
                int indicesArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, IVertexData
                where TIndexType : struct
        {
            int sides = ringSettings.sides;
            int segments = extrusionSettings.segments;
            float2 range = extrusionSettings.range;
            bool capped = ringSettings.capped;

            GetVertexAndIndexCount(extrusionSettings, ringSettings, out var vertexCount, out var indexCount);

            if (sides < 3)
                throw new ArgumentOutOfRangeException(nameof(sides), "Sides must be greater than 3");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 2");
            
            if (vertices.Length < vertexCount)
                throw new ArgumentOutOfRangeException($"Vertex array is incorrect size. Expected {vertexCount} or more, but received {vertices.Length}.");

            if (indices.Length < indexCount)
                throw new ArgumentOutOfRangeException($"Index array is incorrect size. Expected {indexCount} or more, but received {indices.Length}.");

            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, sides, capped, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, sides, capped, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }

            {
                using NativeSpline nativeSpline = new NativeSpline(spline, Allocator.TempJob);
                ExtrudeRingJob<TVertexType> job = new ExtrudeRingJob<TVertexType>()
                {
                    Spline = nativeSpline,
                    Vertices = vertices,
                    ExtrusionSettings = extrusionSettings,
                    RingSettings = ringSettings,
                    VertexArrayOffset = vertexArrayOffset
                };
                job.Schedule(segments, (int)math.sqrt(segments)).Complete();
            }
            
            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * sides;
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * sides;

                var rng = spline.Closed ? math.frac(range) : math.clamp(range, 0f, 1f);
                
                ExtrudeRing(spline, vertices, ExtrudeInfo.StartCap(extrusionSettings, capVertexStart), ringSettings);
                ExtrudeRing(spline, vertices, ExtrudeInfo.EndCap(extrusionSettings, capVertexStart), ringSettings);

                var beginAccel = math.normalize(spline.EvaluateTangent(rng.x));
                var accelLen = math.lengthsq(beginAccel);
                if (accelLen == 0f || float.IsNaN(accelLen))
                    beginAccel = math.normalize(spline.EvaluateTangent(rng.x + 0.0001f));
                var endAccel = math.normalize(spline.EvaluateTangent(rng.y));
                accelLen = math.lengthsq(endAccel);
                if (accelLen == 0f || float.IsNaN(accelLen))
                    endAccel = math.normalize(spline.EvaluateTangent(rng.y - 0.0001f));

                var rad = math.radians(360f / sides);
                var off = new float2(.5f, .5f);

                for (int i = 0; i < sides; ++i)
                {
                    var v0 = vertices[capVertexStart + i];
                    var v1 = vertices[endCapVertexStart + i];

                    v0.normal = -beginAccel;
                    v0.texture = off + new float2(math.cos(i * rad), math.sin(i * rad)) * .5f;

                    v1.normal = endAccel;
                    v1.texture = off + new float2(-math.cos(i * rad), math.sin(i * rad)) * .5f;

                    vertices[capVertexStart + i] = v0;
                    vertices[endCapVertexStart + i] = v1;
                }
            }
        }
        
        
        static void Extrude<TSplineType, TVertexType, TIndexType>(
                TSplineType spline,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                ExtrusionSettings extrusionSettings,
                RingSettings ringSettings,
                int vertexArrayOffset = 0,
                int indicesArrayOffset = 0)
                where TSplineType : ISpline
                where TVertexType : struct, IVertexData
                where TIndexType : struct
        {
            int sides = ringSettings.sides;
            int segments = extrusionSettings.segments;
            float2 range = extrusionSettings.range;
            bool capped = ringSettings.capped;

            GetVertexAndIndexCount(extrusionSettings, ringSettings, out var vertexCount, out var indexCount);

            if (sides < 3)
                throw new ArgumentOutOfRangeException(nameof(sides), "Sides must be greater than 3");

            if (segments < 2)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than 2");
            
            if (vertices.Length < vertexCount)
                throw new ArgumentOutOfRangeException($"Vertex array is incorrect size. Expected {vertexCount} or more, but received {vertices.Length}.");

            if (indices.Length < indexCount)
                throw new ArgumentOutOfRangeException($"Index array is incorrect size. Expected {indexCount} or more, but received {indices.Length}.");

            if (typeof(TIndexType) == typeof(UInt16))
            {
                var ushortIndices = indices.Reinterpret<UInt16>();
                WindTris(ushortIndices, extrusionSettings, sides, capped, vertexArrayOffset, indicesArrayOffset);
            }
            else if (typeof(TIndexType) == typeof(UInt32))
            {
                var ulongIndices = indices.Reinterpret<UInt32>();
                WindTris(ulongIndices, extrusionSettings, sides, capped, vertexArrayOffset, indicesArrayOffset);
            }
            else
            {
                throw new ArgumentException("Indices must be UInt16 or UInt32", nameof(indices));
            }
            
            for (int i = 0; i < segments; ++i)
            {
                ExtrudeInfo info = new ExtrudeInfo(extrusionSettings, i, vertexArrayOffset + i * sides);
                ExtrudeRing(spline, vertices, info, ringSettings);
            }

            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * sides;
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * sides;

                var rng = spline.Closed ? math.frac(range) : math.clamp(range, 0f, 1f);
                ExtrudeRing(spline, vertices, ExtrudeInfo.StartCap(extrusionSettings, capVertexStart), ringSettings);
                ExtrudeRing(spline, vertices, ExtrudeInfo.EndCap(extrusionSettings, capVertexStart), ringSettings);

                var beginAccel = math.normalize(spline.EvaluateTangent(rng.x));
                var accelLen = math.lengthsq(beginAccel);
                if (accelLen == 0f || float.IsNaN(accelLen))
                    beginAccel = math.normalize(spline.EvaluateTangent(rng.x + 0.0001f));
                var endAccel = math.normalize(spline.EvaluateTangent(rng.y));
                accelLen = math.lengthsq(endAccel);
                if (accelLen == 0f || float.IsNaN(accelLen))
                    endAccel = math.normalize(spline.EvaluateTangent(rng.y - 0.0001f));

                var rad = math.radians(360f / sides);
                var off = new float2(.5f, .5f);

                for (int i = 0; i < sides; ++i)
                {
                    var v0 = vertices[capVertexStart + i];
                    var v1 = vertices[endCapVertexStart + i];

                    v0.normal = -beginAccel;
                    v0.texture = off + new float2(math.cos(i * rad), math.sin(i * rad)) * .5f;

                    v1.normal = endAccel;
                    v1.texture = off + new float2(-math.cos(i * rad), math.sin(i * rad)) * .5f;

                    vertices[capVertexStart + i] = v0;
                    vertices[endCapVertexStart + i] = v1;
                }
            }
        }
        
        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        // Note this winds the tris of the whole extruded spline mesh and should work with any extruded shape
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt16> indices, ExtrusionSettings settings, int sides, bool capped, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;

            // loop over the segments along the length of the spline 
            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                // loop over the sides of a given spline extrusion segments
                for (int n = 0; n < sides; ++n)
                {
                    // takes two vertices of the current segment and the two corresponding vertices of the next segment
                    var index0 = vertexArrayOffset + i * sides + n;
                    var index1 = vertexArrayOffset + i * sides + ((n + 1) % sides);
                    var index2 = vertexArrayOffset + ((i+1) % segments) * sides + n;
                    var index3 = vertexArrayOffset + ((i+1) % segments) * sides + ((n + 1) % sides);

                    // for a face we have two triangles, therefore we times the i sides and n faces 
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 0] = (UInt16) index0;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 1] = (UInt16) index1;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 2] = (UInt16) index2;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 3] = (UInt16) index1;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 4] = (UInt16) index3;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 5] = (UInt16) index2;
                }
            }

            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * sides;
                var capIndexStart = indexArrayOffset + sides * 6 * (segments-1);
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * sides;
                var endCapIndexStart = indexArrayOffset + (segments-1) * 6 * sides + (sides-2) * 3;

                for(ushort i = 0; i < sides - 2; ++i)
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
        internal static void WindTris(NativeArray<UInt32> indices, ExtrusionSettings settings, int sides, bool capped, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;

            for (int i = 0; i < (closed ? segments : segments - 1); ++i)
            {
                for (int n = 0; n < sides; ++n)
                {
                    var index0 = vertexArrayOffset + i * sides + n;
                    var index1 = vertexArrayOffset + i * sides + ((n + 1) % sides);
                    var index2 = vertexArrayOffset + ((i+1) % segments) * sides + n;
                    var index3 = vertexArrayOffset + ((i+1) % segments) * sides + ((n + 1) % sides);

                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 0] = (UInt32) index0;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 1] = (UInt32) index1;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 2] = (UInt32) index2;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 3] = (UInt32) index1;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 4] = (UInt32) index3;
                    indices[indexArrayOffset + i * sides * 6 + n * 6 + 5] = (UInt32) index2;
                }
            }

            if (capped)
            {
                var capVertexStart = vertexArrayOffset + segments * sides;
                var capIndexStart = indexArrayOffset + sides * 6 * (segments-1);
                var endCapVertexStart = vertexArrayOffset + (segments + 1) * sides;
                var endCapIndexStart = indexArrayOffset + (segments-1) * 6 * sides + (sides-2) * 3;

                for(ushort i = 0; i < sides - 2; ++i)
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