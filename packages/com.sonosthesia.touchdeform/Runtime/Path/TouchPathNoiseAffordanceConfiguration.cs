using System;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.TouchDeform
{
    [Serializable]
    public class TouchEnvelopeSettings
    {
        [SerializeField] private FloatTouchExtractorSettings _valueExtractor;
        [SerializeField] private FloatTouchExtractorSettings _timeExtractor;
    }
    
    public class TouchPathNoiseAffordanceConfiguration : ScriptableObject
    {
        [SerializeField] private FloatTouchExtractorSettings _radiusExtractor;
        [SerializeField] private FloatTouchExtractorSettings _displacementExtractor;
    }
}