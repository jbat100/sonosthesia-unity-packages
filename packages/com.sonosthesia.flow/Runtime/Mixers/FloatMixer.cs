using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sonosthesia.Signal
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

        [SerializeField] private Strategy _strategy;

        protected override float Mix(IEnumerable<float> values) => _strategy switch
        {
            Strategy.Add => values.Sum(),
            Strategy.Multiply => values.Aggregate(1.0f, (current, val) => current * val),
            Strategy.Min => values.Min(),
            Strategy.Max => values.Max(),
            _ => 0
        };
    }
}