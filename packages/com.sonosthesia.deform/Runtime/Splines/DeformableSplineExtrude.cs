using UnityEngine;
using UnityEngine.Splines;
using Sonosthesia.Mesh;

namespace Sonosthesia.Deform
{
    public abstract class DeformableSplineExtrude : SplineRingExtrude
    {
        [SerializeField] private bool m_BypassDeformation;
        
        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel)
        {
            base.PopulateMeshData(data, spline, extrusionSettings, parallel);

            if (!m_BypassDeformation)
            {
                Deform(spline, data, RingSettings, extrusionSettings);   
            }
        }

        protected abstract void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, SplineRingExtrusion.RingSettings ringSettings, ExtrusionSettings extrusionSettings);

    }
}