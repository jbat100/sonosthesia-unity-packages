using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Serializable]
    public class OneEuroFilterSettings
    {
        [SerializeField] private float _beta = 0.1f;
        [SerializeField] private float _minCutoff = 0.1f;

        public void ApplyTo(IOneEuroFilterParameters parameters)
        {
            parameters.Beta = _beta;
            parameters.MinCutoff = _minCutoff;
        }
    }

    public static class OneEuroFilterSettingsExtensions
    {
        public static void Apply(this IOneEuroFilterParameters parameters, OneEuroFilterSettings settings)
        {
            settings.ApplyTo(parameters);
        }
    }
}