using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SplineVertexData : SplineMesh.ISplineVertexData
    {
        public Vector3 position { get; set; }
        public Vector3 normal { get; set; } 
        public Vector2 texture { get; set; }
    }

    // The logic around when caps and closing is a little complicated and easy to confuse. This wraps settings in a
    // consistent way so that methods aren't working with mixed data.
    public struct ExtrusionSettings
    {
        const int k_SegmentsMin = 2, k_SegmentsMax = 4096;
            
        public int segments { get; private set; }
        public bool capped { get; private set; }
        public bool closed { get; private set; }
        public float2 range { get; private set; }

        public ExtrusionSettings(int segments, bool capped, bool closed, float2 range)
        {
            this.segments = math.clamp(segments, k_SegmentsMin, k_SegmentsMax);
            this.range = new float2(math.min(range.x, range.y), math.max(range.x, range.y));
            this.closed = math.abs(1f - (this.range.y - this.range.x)) < float.Epsilon && closed;
            this.capped = capped && !this.closed;
        }
    }
    
    public static class SplineExtrusion
    {
        // Two overloads for winding triangles because there is no generic constraint for UInt{16, 32}
        // Note this winds the tris of the whole extruded spline mesh and should work with any extruded shape
        [BurstCompile]
        internal static void WindTris(NativeArray<UInt16> indices, ExtrusionSettings settings, int sides, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;
            var capped = settings.capped;

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
        internal static void WindTris(NativeArray<UInt32> indices, ExtrusionSettings settings, int sides, int vertexArrayOffset = 0, int indexArrayOffset = 0)
        {
            var closed = settings.closed;
            var segments = settings.segments;
            var capped = settings.capped;

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