using UnityEngine;

namespace Sonosthesia.Flow
{
    public class StaticColorModulator : BlendColorModulator
    {
        [SerializeField] private Color _value;

        protected override Color Modulate(float offset) => _value;
    }
}