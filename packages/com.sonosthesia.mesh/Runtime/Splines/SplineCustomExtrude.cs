using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    public abstract class SplineCustomExtrude : MeshController
    {
        [SerializeField] private SplineReference _spline;

        [SerializeField] private float _segmentsPerUnit = 4;
        
        [SerializeField] private float _scale = 1f;
        
        [SerializeField] private float _fade = 0f;

        [SerializeField] private ExtrusionVStrategy m_VStrategy = ExtrusionVStrategy.NormalizedRange;
        
        [SerializeField] private Vector2 _range = new Vector2(0f, 1f);

        [SerializeField] private bool _parallel;

        protected virtual void Reset()
        {
            // TODO : consider nuking
            
            if (TryGetComponent<MeshRenderer>(out var renderer) && renderer.sharedMaterial == null)
            {
                // todo Make Material.GetDefaultMaterial() public
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var mat = cube.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(cube);
                renderer.sharedMaterial = mat;
            }

            Rebuild();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Spline.Changed += OnSplineChanged;
        }

        protected virtual void OnDisable()
        {
            Spline.Changed -= OnSplineChanged;
        }

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
        {
            if (_spline?.Spline == spline)
            {
                RequestRebuild();
            }
        }
        
        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data, Spline spline, ExtrusionSettings extrusionSettings, bool parallel);
        
        /// <summary>
        /// Triggers the rebuild of a Spline's extrusion mesh and collider.
        /// </summary>
        protected override void PopulateMeshData(UnityEngine.Mesh.MeshData data)
        {
            Spline spline = _spline.Spline;
            if (spline == null)
            {
                return;
            }

            float length = spline.GetLength();
            float span = Mathf.Abs(_range.y - _range.x);
            int segments = Mathf.Max((int)Mathf.Ceil(length * span * _segmentsPerUnit), 1);
            
            // Debug.Log($"{this} rebuilding spline with length : {splineLength}, range {m_Range}, span {span}, segments {segments}");

            ExtrusionSettings extrusionSettings = new ExtrusionSettings(length, segments, spline.Closed, _range, _scale, _fade, m_VStrategy);

            PopulateMeshData(data, spline, extrusionSettings, _parallel);
        }
    }
}