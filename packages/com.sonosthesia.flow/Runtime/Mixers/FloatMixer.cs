using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class FloatMixer : Mixer<float>
    {
        public enum Strategy
        {
            None,
            Add,
            Multiply,
            Max,
            Min
        }

        [SerializeField] private Strategy _strategy = Strategy.Max;

        [SerializeField] private FloatProcessor _postProcessor;

        protected override float Mix(IEnumerable<float> values)
        {
            float raw = _strategy switch
            {
                Strategy.Add => values.Sum(),
                Strategy.Multiply => values.Aggregate(1.0f, (current, val) => current * val),
                Strategy.Min => values.Min(),
                Strategy.Max => values.Max(),
                _ => 0
            };

            return _postProcessor.Process(raw);
        }
    }
}