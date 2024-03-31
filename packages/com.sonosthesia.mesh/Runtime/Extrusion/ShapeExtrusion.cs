using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public struct ShapeSettings
    {
        const float k_ScaleMin = .00001f, k_ScaleMax = 10000f;

        public int points { get; private set; }

        public bool closed { get; private set; }

        public int Sides => Mathf.Max(closed ? points : points - 1, 0);

        public ShapeSettings(int points, bool closed)
        {
            this.points = points;
            this.closed = closed;
        }
    }
    
    public static class ShapeExtrusion
    {
        
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