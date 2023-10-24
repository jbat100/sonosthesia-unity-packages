using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public class StaticFloatModulator : BlendFloatModulator
    {
        [SerializeField] private float _value;

        protected override float Modulate(float offset) => _value;
    }
}