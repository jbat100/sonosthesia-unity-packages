using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Mesh
{
    public class PathFaceExtrude : PathCustomExtrude
    {
        [SerializeField] private ExtrusionSegmentContainer _segmentContainer;

        private NativeArray<ExtrusionSegment> _extrusionSegments;

        protected override void OnDisable()
        {
            base.OnDisable();
            _extrusionSegments.Dispose();
        }

        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, NativeArray<RigidTransform> pathPoints, ExtrusionSettings extrusionSettings, bool parallel)
        {
            data.subMeshCount = 1;

            _segmentContainer.Populate(ref _extrusionSegments);

            FaceSettings faceSettings = new FaceSettings(_extrusionSegments.Length);
            
            FaceExtrusion.GetVertexAndIndexCount(extrusionSettings, faceSettings, out int vertexCount, out int indexCount);

            IndexFormat indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;

            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, Extrusion.PipeVertexAttribs);

            NativeArray<VertexData> vertices = data.GetVertexData<VertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                FaceExtrusion.Extrude(parallel, pathPoints, vertices, indices, _extrusionSegments, extrusionSettings, faceSettings);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                FaceExtrusion.Extrude(parallel, pathPoints, vertices, indices, _extrusionSegments, extrusionSettings, faceSettings);
            }

            data.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
        }
    }
}