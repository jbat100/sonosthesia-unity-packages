using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold
{
    public class BiSplineOutline : MonoBehaviour
    {
        [SerializeField] private SplineContainer _guideSplineContainer;

        [SerializeField] private int _guideSplineIndex;
        
        [SerializeField] private SplineContainer _orientationSplineContainer;

        [SerializeField] private int _orientationSplineIndex;
        
        protected virtual void Awake()
        {
            Spline.Changed += SplineOnChanged;
        }

        protected virtual void OnDestroy()
        {
            Spline.Changed -= SplineOnChanged;
        }
        
        private void SplineOnChanged(Spline arg1, int arg2, SplineModification arg3)
        {
            throw new System.NotImplementedException();
        }
        
    }    
}


