using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    public class SplineFaceExtrude : SplineCustomExtrude
    {
        [SerializeField] 
        private ExtrusionSegmentContainer _segmentContainer;

        private NativeArray<ExtrusionSegment> _segments;

        protected override void OnDisable()
        {
            base.OnDisable();
            _segments.Dispose();
        }
        
        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel)
        {
            if (!_segmentContainer)
            {
                return;
            }
            
            FaceSettings faceSettings = new (_segmentContainer.SegmentCount);

            Extrude(spline, data, _segmentContainer, extrusionSettings, faceSettings, parallel);
        }

        private void Extrude<TSpline>(TSpline spline, UnityEngine.Mesh.MeshData data, ExtrusionSegmentContainer segmentContainer,
            ExtrusionSettings extrusionSettings, FaceSettings faceSettings, bool parallel)
            where TSpline : ISpline
        {
            data.subMeshCount = 1;

            segmentContainer.Populate(ref _segments);
            
            FaceExtrusion.GetVertexAndIndexCount(extrusionSettings, faceSettings, out int vertexCount, out int indexCount);

            IndexFormat indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;

            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, Extrusion.PipeVertexAttribs);

            NativeArray<VertexData> vertices = data.GetVertexData<VertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                SplineFaceExtrusion.Extrude(parallel, spline, vertices, indices, _segments, extrusionSettings, faceSettings);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                SplineFaceExtrusion.Extrude(parallel, spline, vertices, indices, _segments, extrusionSettings, faceSettings);
            }

            data.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
        }
    }
}