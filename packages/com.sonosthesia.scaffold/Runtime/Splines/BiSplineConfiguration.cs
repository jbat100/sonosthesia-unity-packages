using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold
{
    public class BiSplineConfiguration : ObservableBehaviour
    {
        [SerializeField] private SplineReference _guide;
        public SplineReference Guide => _guide;
        
        [SerializeField] private SplineReference _orientation;
        public SplineReference Orientation => _orientation;

        public BiSplineVertices Vertices
        {
            get
            {
                Spline guide = _guide.Spline;
                Spline orientation = _orientation.Spline;
                
                Vector3 guideStart = guide.EvaluatePosition(0);
                Vector3 guideEnd = guide.EvaluatePosition(1);
                Vector3 orientationStart = orientation.EvaluatePosition(0);
                Vector3 orientationEnd = orientation.EvaluatePosition(1);
            
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
            if (_guide.Spline == spline || _orientation.Spline == spline)
            {
                BroadcastChange();
            }
        }
        
    }
}