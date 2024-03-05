using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class PeakSelector : Selector<Peak>
    {
        private enum Selection
        {
            None,
            Magnitude,
            Duration
        }

        [SerializeField] private Selection _selection = Selection.Magnitude;
        
        protected override float InternalSelect(Peak value)
        {
            return _selection switch
            {
                Selection.Magnitude => value.Magnitude,
                Selection.Duration => value.Duration,
                _ => 0
            };
        }
    }
}