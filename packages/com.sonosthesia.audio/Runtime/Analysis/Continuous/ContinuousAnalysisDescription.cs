using System;
using UnityEngine;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class ContinuousAnalysisDescription
    {
        [Serializable]
        class MagnitudeRange
        {
            [SerializeField] private float _lower;
            [SerializeField] private float _upper;

            public override string ToString()
            {
                return $"Range ({_lower:F2} : {_upper:F2}) dB";
            }
        }

        [SerializeField] private MagnitudeRange _mainRange;
        [SerializeField] private MagnitudeRange _lowRange;
        [SerializeField] private MagnitudeRange _midRange;
        [SerializeField] private MagnitudeRange _highRange;


    }
}