using System;
using UnityEngine;
using Unity.Mathematics;

namespace Sonosthesia.Audio
{
    // note: magnitudes in analysis files are expressed in dBs which range from -100 to 0. Typically
    // - only a subset of this range is of interest to drive processes (around -40 to -5)
    // - it's easier for clients to assume a normalized value 
    // this domain specific processor clips to a specified range and normalizes the result
    
    [Serializable]
    public class AnalysisDBPreprocessor
    {
        [SerializeField] private float _dBLowerThreshold = -40;
        [SerializeField] private float _dBUpperThreshold = -5;
        [SerializeField] private bool _clip = true;
        [SerializeField] private bool _normalize = true;

        public float Process(float value)
        {
            if (_clip)
            {
                value = math.clamp(value, _dBLowerThreshold, _dBUpperThreshold);
            }

            if (_normalize)
            {
                value = math.remap(_dBLowerThreshold, _dBUpperThreshold, 0f, 1f, value);
            }

            return value;
        }
    }
}