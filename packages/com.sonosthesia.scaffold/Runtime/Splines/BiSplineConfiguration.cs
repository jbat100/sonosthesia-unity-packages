using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold
{
    public class BiSplineConfiguration : ObservableBehaviour
    {
        [SerializeField] private SplineContainer _guideSplineContainer;

        [SerializeField] private int _guideSplineIndex;
        
        [SerializeField] private SplineContainer _orientationSplineContainer;

        [SerializeField] private int _orientationSplineIndex;
        
        public Spline GuideSpline => _guideSplineContainer[_guideSplineIndex];
        
        public Spline OrientationSpline => _orientationSplineContainer[_orientationSplineIndex];

        public BiSplineVertices Vertices
        {
            get
            {
                Vector3 guideStart = GuideSpline.EvaluatePosition(0);
                Vector3 guideEnd = GuideSpline.EvaluatePosition(1);
                Vector3 orientationStart = OrientationSpline.EvaluatePosition(0);
                Vector3 orientationEnd = OrientationSpline.EvaluatePosition(1);
            
                // force coplanar

                Plane plane = new Plane(guideStart, guideEnd, orientationStart);
                orientationEnd = plane.ClosestPointOnPlane(orientationEnd);
                
                Vector3 p0 = orientationEnd;
                Vector3 p1 = orientationStart;
            
                // reflect about guide line https://stackoverflow.com/questions/48793217/how-can-i-reflect-a-point-about-a-line-in-unity

                Vector3 guideDirection = (guideEnd - guideStart).normalized;
                Vector3 p0Direction = guideStart - p0;
                Vector3 p1Direction = guideStart - p1;
                Vector3 p2 = Vector3.Reflect(p1Direction, guideDirection);
                Vector3 p3 = Vector3.Reflect(p0Direction, guideDirection);

                return new BiSplineVertices(p0, p1, p2, p3);
            }
        }

        protected virtual void Awake()
        {
            Spline.Changed += SplineOnChanged;
        }

        protected virtual void OnDestroy()
        {
            Spline.Changed -= SplineOnChanged;
        }
        
        private void SplineOnChanged(Spline spline, int knot, SplineModification modification)
        {
            Debug.Log($"{this} {nameof(SplineOnChanged)}");
            if (GuideSpline == spline || OrientationSpline == spline)
            {
                BroadcastChange();
            }
        }
        
    }
}