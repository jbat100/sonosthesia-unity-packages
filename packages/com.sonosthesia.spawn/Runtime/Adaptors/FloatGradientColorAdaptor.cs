using UnityEngine;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class FloatGradientColorAdaptor : FloatMapAdaptor<Color>
    {
        [SerializeField] private Gradient _gradient;

        protected override Color Map(float value) => _gradient.Evaluate(value);
    }
}