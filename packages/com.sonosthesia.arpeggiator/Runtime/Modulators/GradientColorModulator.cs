using UnityEngine;

namespace Sonosthesia.Flow
{
    public class GradientColorModulator : BlendColorModulator
    {
        [SerializeField] private Gradient _gradient;

        protected override Color Modulate(float offset) => _gradient.Evaluate(offset);
    }
}