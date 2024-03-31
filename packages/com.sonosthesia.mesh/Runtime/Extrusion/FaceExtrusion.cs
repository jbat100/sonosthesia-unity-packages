using System;
using Unity.Burst;
using Unity.Collections;

namespace Sonosthesia.Mesh
{
    public struct FaceSettings
    {
        public int faces { get; private set; }

        public FaceSettings(int faces)
        {
            this.faces = faces;
        }
    }
    
    public static class FaceExtrusion
    {
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