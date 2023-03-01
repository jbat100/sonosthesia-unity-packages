using UnityEngine;

namespace Sonosthesia.Flow
{
    public class StaticFloatModulator : BlendFloatModulator
    {
        [SerializeField] private float _value;

        protected override float Modulate(float offset) => _value;
    }
}