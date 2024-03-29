using UnityEngine;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class FloatGradientSpeedColorAdaptor : SpeedFloatAdaptor<Color>
    {
        [SerializeField] private Gradient _gradient;

        protected override Color Map(float value) => _gradient.Evaluate(value);
    }
}