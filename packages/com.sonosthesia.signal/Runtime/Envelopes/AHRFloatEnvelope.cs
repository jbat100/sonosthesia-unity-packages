using UnityEngine;

namespace Sonosthesia.Flow
{
    public class AHRFloatEnvelope : FloatEnvelope
    {
        [SerializeField] private float _attack;
        [SerializeField] private float _hold;
        [SerializeField] private float _release;
        [SerializeField] private float _amplitude;
        
        public override float Duration()
        {
            return _attack + _hold + _release;
        }

        public override float Evaluate(float t)
        {
            if (t >= 0 && t < _attack)
            {
                return t  * _amplitude / _attack;
            }
            if (t >= _attack && t < _attack + _hold)
            {
                return _amplitude;
            }
            if (t >= _attack + _hold && t < _attack + _hold + _release)
            {
                return (1 - ((t - (_attack + _hold)) / _release)) * _amplitude;
            }
            return 0;
        }
    }
}