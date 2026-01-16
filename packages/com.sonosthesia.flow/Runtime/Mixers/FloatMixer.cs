using System.Collections.Generic;
using Sonosthesia.Processing;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class FloatMixer : Mixer<float>
    {
        [SerializeField] private FloatModulationStrategy _strategy = FloatModulationStrategy.Max;

        [SerializeField] private FloatProcessorSettings _postProcessor;

        protected override float Mix(IEnumerable<float> values)
        {
            float raw = _strategy.Modulate(values);
            return _postProcessor.Process(raw);
        }
    }
}