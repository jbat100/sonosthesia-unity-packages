using UnityEngine;

namespace Sonosthesia.Scaffold
{
    [ExecuteAlways]
    public class BiSplineLine : BiSplineAffordance
    {
        [SerializeField] private LineRenderer _lineRenderer;

        [SerializeField] private bool _applyTransform;
        
        protected override void Setup()
        {
            if (!_lineRenderer)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }

            if (_lineRenderer)
            {
                _lineRenderer.positionCount = 4;
                _lineRenderer.loop = true;
                _lineRenderer.useWorldSpace = _applyTransform;    
            }
            
            base.Setup();
        }
        
        protected override void RefreshAffordance()
        {
            _lineRenderer.positionCount = 4;
            
            BiSplineVertices vertices = Configuration.Vertices;

            Vector3[] positions = new Vector3[4];

            Vector3 Process(Vector3 point)
            {
                return _applyTransform ? transform.TransformPoint(point) : point;
            }
            
            positions[0] = Process(vertices.P0);
            positions[1] = Process(vertices.P1);
            positions[2] = Process(vertices.P2);
            positions[3] = Process(vertices.P3);
            
            _lineRenderer.SetPositions(positions);
            
        }
    }
}