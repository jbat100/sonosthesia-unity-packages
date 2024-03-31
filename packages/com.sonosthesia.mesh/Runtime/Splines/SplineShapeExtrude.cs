using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    public class SplineShapeExtrude : SplineCustomExtrude
    {
        [SerializeField] 
        private ExtrusionPointContainer m_PointContainer;

        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel)
        {
            if (!m_PointContainer)
            {
                return;
            }
            
            ShapeSettings shapeSettings = new ShapeSettings(m_PointContainer.PointCount, m_PointContainer.Closed);

            Extrude(spline, data, m_PointContainer, extrusionSettings, shapeSettings, parallel);
        }

        private void Extrude<TSpline>(TSpline spline, UnityEngine.Mesh.MeshData data, ExtrusionPointContainer pointContainer,
            ExtrusionSettings extrusionSettings, ShapeSettings shapeSettings, bool parallel)
            where TSpline : ISpline
        {
            data.subMeshCount = 1;

            NativeArray<ExtrusionPoint> points = pointContainer.NativePoints;

            SplineShapeExtrusion.GetVertexAndIndexCount(extrusionSettings, shapeSettings, out int vertexCount, out int indexCount);

            IndexFormat indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;

            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, Extrusion.PipeVertexAttribs);

            NativeArray<VertexData> vertices = data.GetVertexData<VertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                // Debug.Log($"{this} expecting {vertexCount} vertices (array count is {vertices.Length}) and {indexCount} indices (array count is {indices.Length})");
                SplineShapeExtrusion.Extrude(parallel, spline, vertices, indices, points, extrusionSettings, shapeSettings);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                // Debug.Log($"{this} expecting {vertexCount} vertices (array count is {vertices.Length}) and {indexCount} indices (array count is {indices.Length})");
                SplineShapeExtrusion.Extrude(parallel, spline, vertices, indices, points, extrusionSettings, shapeSettings);
            }

            data.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
        }
    }
}