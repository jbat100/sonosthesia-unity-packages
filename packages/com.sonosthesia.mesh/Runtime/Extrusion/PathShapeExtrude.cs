using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public class PathShapeExtrude : PathCustomExtrude
    {
        [SerializeField] private ExtrusionPointContainer _pointContainer;

        private NativeArray<ExtrusionPoint> _extrusionPoints;

        protected override void OnDisable()
        {
            base.OnDisable();
            _extrusionPoints.Dispose();
        }
        
        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data, NativeArray<RigidTransform> pathPoints, ExtrusionSettings extrusionSettings, bool parallel)
        {
            
        }
    }
}