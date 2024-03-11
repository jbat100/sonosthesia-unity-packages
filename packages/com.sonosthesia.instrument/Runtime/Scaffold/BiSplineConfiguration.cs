using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Instrument
{
    public class BiSplineConfiguration : ObservableBehaviour
    {
        [SerializeField] private SplineContainer _guideSplineContainer;

        [SerializeField] private int _guideSplineIndex;
        
        [SerializeField] private SplineContainer _orientationSplineContainer;

        [SerializeField] private int _orientationSplineIndex;
        
        private Spline GuideSpline => _guideSplineContainer[_guideSplineIndex];
        
        private Spline OrientationSpline => _orientationSplineContainer[_orientationSplineIndex];
        
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