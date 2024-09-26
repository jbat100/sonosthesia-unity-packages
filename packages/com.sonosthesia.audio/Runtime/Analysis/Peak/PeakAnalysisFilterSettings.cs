using System;
using UnityEngine;

namespace Sonosthesia.Audio
{
    [Serializable]
    public class PeakAnalysisFilterSettings
    {
        [SerializeField] private float _magnitudeThreshold = -30;

        [SerializeField] private float _strengthThreshold = 0;

        public bool Check(PeakAnalysis peakAnalysis)
        {
            return peakAnalysis.strength > _strengthThreshold && peakAnalysis.magnitude > _magnitudeThreshold;
        }
    }
}