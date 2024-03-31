using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public readonly struct FaceSettings
    {
        public readonly int faces;

        public FaceSettings(int faces)
        {
            this.faces = faces;
        }
    }
    
    public static class FaceExtrusion
    {
        public static void Extrude<TVertexType, TIndexType>(
            bool parallel,
            NativeArray<RigidTransform> pathPoints,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionSegment> segments,
            ExtrusionSettings extrusionSettings,
            FaceSettings faceSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TVertexType : struct, IVertexData
            where TIndexType : struct
        {
            if (parallel)
            {
                ExtrudeParallel(pathPoints, vertices, indices, segments,
                    extrusionSettings, faceSettings,
                    vertexArrayOffset, indicesArrayOffset);  
            }
            else
            {
                Extrude(pathPoints, vertices, indices, segments,
                    extrusionSettings, faceSettings,
                    vertexArrayOffset, indicesArrayOffset);    
            }
        }
        
        [BurstCompile]
        private struct ExtrudeFacesJob<K> : IJobParallelFor where K : struct, IVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<RigidTransform> PathPoints;
            
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<ExtrusionSegment> ExtrusionSegments;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;

            public ExtrusionSettings ExtrusionSettings;
            public FaceSettings FaceSettings;
            public int VertexArrayOffset;

            public void Execute(int index)
            {
                RigidTransform pathPoint = PathPoints[index];
                ExtrudeSegments(pathPoint, Vertices, ExtrusionSegments, ExtrusionSettings, index, VertexArrayOffset);
            }
        }
        
        public static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, FaceSettings faceSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = faceSettings.faces * 2 * extrusionSettings.segments;
            indexCount = faceSettings.faces * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1));
        }

        public static void ExtrudeParallel<TVertexType, TIndexType>(
                NativeArray<RigidTransform> pathPoints,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionSegment> extrusionSegments,
                ExtrusionSettings extrusionSettings,
                FaceSettings faceSettings,
                int vertexArrayOffset = 0,
                int indexArrayOffset = 0)
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
                ExtrudeFacesJob<TVertexType> job = new ExtrudeFacesJob<TVertexType>()
                {
                    PathPoints = pathPoints,
                    Vertices = vertices,
                    ExtrusionSegments = extrusionSegments,
                    ExtrusionSettings = extrusionSettings,
                    FaceSettings = faceSettings,
                    VertexArrayOffset = vertexArrayOffset
                };
                job.Schedule(extrusionSettings.segments, (int)math.sqrt(extrusionSettings.segments)).Complete();
            }
        }
        
        public static void Extrude<TVertexType, TIndexType>(
            NativeArray<RigidTransform> pathPoints,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionSegment> extrusionSegments,
            ExtrusionSettings extrusionSettings,
            FaceSettings faceSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
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

            for (int i = 0; i < extrusionSettings.segments; ++i)
            {
                RigidTransform pathPoint = pathPoints[i];
                ExtrudeSegments(pathPoint, vertices, extrusionSegments, extrusionSettings, i, vertexArrayOffset);           
            }
        }
        
        [BurstCompile]
        private static void ExtrudeSegments<K>(RigidTransform pathPoint, NativeArray<K> vertices,
            NativeArray<ExtrusionSegment> extrusionSegments, ExtrusionSettings extrusionSettings, 
            int index, int vertexArrayOffset) where K : struct, IVertexData
        {
            ExtrudeInfo info = new ExtrudeInfo(extrusionSettings, index, vertexArrayOffset + index * extrusionSegments.Length * 2);
            ExtrudeSegments(pathPoint, info, vertices, extrusionSegments);
        }

        [BurstCompile]
        private static void ExtrudeSegments<K>(RigidTransform pathPoint, ExtrudeInfo info, NativeArray<K> vertices, NativeArray<ExtrusionSegment> segments)
            where K : struct, IVertexData
        {
            int count = segments.Length;
            
            for (int n = 0; n < count; ++n)
            {
                ExtrusionSegment segment = segments[n];

                K MakeVertex(ExtrusionPoint extrusionPoint)
                {
                    K vertex = new K();
                
                    Vector3 position = extrusionPoint.position * info.scale;
                    vertex.position = pathPoint.pos + math.rotate(pathPoint.rot, position);
                
                    Vector3 normal = extrusionPoint.normal;
                    vertex.normal = math.rotate(pathPoint.rot, normal);
                
                    vertex.texture = new Vector2(extrusionPoint.u, info.v);

                    return vertex;
                }

                int indexOffset = info.start + 2 * n;

                vertices[indexOffset] = MakeVertex(segment.start);
                vertices[indexOffset + 1] = MakeVertex(segment.end);
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