using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold
{
    public class BiSplineContour : BiSplineAffordance
    {
        [SerializeField] private SplineContainer _targetSplineContainer;

        [SerializeField] private int _targetSplineIndex;

        [SerializeField] private TangentMode _tangentMode = TangentMode.AutoSmooth;

        protected override void RefreshAffordance()
        {
            Spline spline = _targetSplineContainer[_targetSplineIndex];
            
            spline.Clear();

            BiSplineVertices splineVertices = Configuration.Vertices;

            void AddPoint(Vector3 point)
            {
                BezierKnot knot = new BezierKnot(point);
                spline.Add(knot, _tangentMode);
            }
            
            // note we could also add all the knots in the orientation spline forward then backward
            
            AddPoint(splineVertices.P0);
            AddPoint(splineVertices.P1);
            AddPoint(splineVertices.P2);
            AddPoint(splineVertices.P3);
            
            spline.Closed = true;
        }
    }
}