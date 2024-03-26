using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Sonosthesia.Mesh;

namespace Sonosthesia.Deform
{
    public abstract class DeformableSplineExtrude : SplineCustomExtrude
    {
        [SerializeField, Tooltip("The radius of the extruded mesh.")]
        private float m_Radius = .25f;
        
        [SerializeField, Tooltip("Sides of the extrusion circle.")]
        private int m_Sides = 10;

        [SerializeField] private bool m_BypassDeformation;
        
        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel)
        {
            SplineRingExtrusion.RingSettings ringSettings = new SplineRingExtrusion.RingSettings(m_Sides, m_Radius);
            
            Extrude(spline, data, ringSettings, extrusionSettings, parallel);

            if (!m_BypassDeformation)
            {
                Deform(spline, data, ringSettings, extrusionSettings);   
            }
        }

        static readonly VertexAttributeDescriptor[] k_PipeVertexAttribs = new VertexAttributeDescriptor[]
        {
            new (VertexAttribute.Position),
            new (VertexAttribute.Normal),
            new (VertexAttribute.TexCoord0, dimension: 2)
        };
        
        private void Extrude<TSpline>(TSpline spline, UnityEngine.Mesh.MeshData data, SplineRingExtrusion.RingSettings ringSettings, ExtrusionSettings extrusionSettings, bool parallel) 
            where TSpline : ISpline
        {
            data.subMeshCount = 1;
            
            SplineMesh.GetVertexAndIndexCount(ringSettings.sides, extrusionSettings.segments, extrusionSettings.capped, spline.Closed, extrusionSettings.range, out int vertexCount, out int indexCount);
            
            var indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            
            data.SetIndexBufferParams(indexCount, indexFormat);
            data.SetVertexBufferParams(vertexCount, k_PipeVertexAttribs);

            NativeArray<SplineVertexData> vertices = data.GetVertexData<SplineVertexData>();
            if (indexFormat == IndexFormat.UInt16)
            {
                NativeArray<UInt16> indices = data.GetIndexData<UInt16>();
                SplineRingExtrusion.Extrude(parallel, spline, vertices, indices, ringSettings, extrusionSettings);
            }
            else
            {
                NativeArray<UInt32> indices = data.GetIndexData<UInt32>();
                SplineRingExtrusion.Extrude(parallel, spline, vertices, indices, ringSettings, extrusionSettings);
            }
            
            data.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
        }

        protected abstract void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, SplineRingExtrusion.RingSettings ringSettings, ExtrusionSettings extrusionSettings);

    }
}