using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.PeakDetector
{
    public class PeakSelector : Selector<Peak>
    {
        private enum Selection
        {
            None,
            Unit,
            Magnitude,
            Duration
        }

        [SerializeField] private Selection _selection = Selection.Magnitude;
        
        protected override float InternalSelect(Peak value)
        {
            return _selection switch
            {
                Selection.Unit => 1f,
                Selection.Magnitude => value.Magnitude,
                Selection.Duration => value.Duration,
                _ => 0
            };
        }
    }
}