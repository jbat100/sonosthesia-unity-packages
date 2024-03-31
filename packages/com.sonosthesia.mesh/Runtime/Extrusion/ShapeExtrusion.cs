using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
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
        public static void Extrude<TVertexType, TIndexType>(
            bool parallel,
            NativeArray<RigidTransform> pathPoints,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionPoint> extrusionPoints,
            ExtrusionSettings extrusionSettings,
            ShapeSettings shapeSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TVertexType : struct, IVertexData
            where TIndexType : struct
        {
            if (parallel)
            {
                ExtrudeParallel(pathPoints, vertices, indices, extrusionPoints,
                    extrusionSettings, shapeSettings,
                    vertexArrayOffset, indicesArrayOffset);  
            }
            else
            {
                Extrude(pathPoints, vertices, indices, extrusionPoints,
                    extrusionSettings, shapeSettings,
                    vertexArrayOffset, indicesArrayOffset);    
            }
        }
        
        [BurstCompile]
        private struct ExtrudeShapeJob<K> : IJobParallelFor where K : struct, IVertexData
        {
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<RigidTransform> PathPoints;
            
            [NativeDisableContainerSafetyRestriction]
            [ReadOnly] public NativeArray<ExtrusionPoint> ExtrusionPoints;
            
            [NativeDisableContainerSafetyRestriction]
            [WriteOnly] public NativeArray<K> Vertices;

            public ExtrusionSettings ExtrusionSettings;
            public ShapeSettings ShapeSettings;
            public int VertexArrayOffset;

            public void Execute(int index)
            {
                RigidTransform pathPoint = PathPoints[index];
                ExtrudePoints(pathPoint, Vertices, ExtrusionPoints, ExtrusionSettings, index, VertexArrayOffset);
            }
        }
        
        public static void GetVertexAndIndexCount(ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, out int vertexCount, out int indexCount)
        {
            vertexCount = shapeSettings.points * extrusionSettings.segments;
            indexCount = shapeSettings.Sides * 6 * (extrusionSettings.segments - (extrusionSettings.closed ? 0 : 1));
        }

        public static void ExtrudeParallel<TVertexType, TIndexType>(
                NativeArray<RigidTransform> pathPoints,
                NativeArray<TVertexType> vertices,
                NativeArray<TIndexType> indices,
                NativeArray<ExtrusionPoint> extrusionPoints,
                ExtrusionSettings extrusionSettings,
                ShapeSettings shapeSettings,
                int vertexArrayOffset = 0,
                int indexArrayOffset = 0)
                where TVertexType : struct, IVertexData
                where TIndexType : struct
        {
            
            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 2");
            
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
                ExtrudeShapeJob<TVertexType> job = new ExtrudeShapeJob<TVertexType>()
                {
                    PathPoints = pathPoints,
                    Vertices = vertices,
                    ExtrusionPoints = extrusionPoints,
                    ExtrusionSettings = extrusionSettings,
                    ShapeSettings = shapeSettings,
                    VertexArrayOffset = vertexArrayOffset
                };
                job.Schedule(extrusionSettings.segments, (int)math.sqrt(extrusionSettings.segments)).Complete();
            }
        }
        
        public static void Extrude<TVertexType, TIndexType>(
            NativeArray<RigidTransform> pathPoints,
            NativeArray<TVertexType> vertices,
            NativeArray<TIndexType> indices,
            NativeArray<ExtrusionPoint> extrusionPoints,
            ExtrusionSettings extrusionSettings,
            ShapeSettings shapeSettings,
            int vertexArrayOffset = 0,
            int indicesArrayOffset = 0)
            where TVertexType : struct, IVertexData
            where TIndexType : struct
        {
            if (extrusionSettings.segments < 2)
                throw new ArgumentOutOfRangeException(nameof(extrusionSettings.segments), "Segments must be greater than 1");
           
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

            for (int i = 0; i < extrusionSettings.segments; ++i)
            {
                RigidTransform pathPoint = pathPoints[i];
                ExtrudePoints(pathPoint, vertices, extrusionPoints, extrusionSettings, i, vertexArrayOffset);           
            }
        }
        
        [BurstCompile]
        private static void ExtrudePoints<K>(RigidTransform pathPoint, NativeArray<K> vertices,
            NativeArray<ExtrusionPoint> extrusionPoints, ExtrusionSettings extrusionSettings, 
            int index, int vertexArrayOffset) where K : struct, IVertexData
        {
            ExtrudeInfo info = new ExtrudeInfo(extrusionSettings, index, vertexArrayOffset + index * extrusionPoints.Length * 2);
            ExtrudePoints(pathPoint, info, vertices, extrusionPoints);
        }

        [BurstCompile]
        private static void ExtrudePoints<K>(RigidTransform pathPoint, ExtrudeInfo info, NativeArray<K> vertices, NativeArray<ExtrusionPoint> extrusionPoints)
            where K : struct, IVertexData
        {
            int count = extrusionPoints.Length;
            
            for (int n = 0; n < count; ++n)
            {
                ExtrusionPoint point = extrusionPoints[n];
                
                var vertex = new K();
                
                Vector3 position = point.position * info.scale;
                vertex.position = pathPoint.pos + math.rotate(pathPoint.rot, position);
                
                Vector3 normal = point.normal;
                vertex.normal = math.rotate(pathPoint.rot, normal);
                
                vertex.texture = new Vector2(point.u, info.v);

                vertices[info.start + n] = vertex;
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