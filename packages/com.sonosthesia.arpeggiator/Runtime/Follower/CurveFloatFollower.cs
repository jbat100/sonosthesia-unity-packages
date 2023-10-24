using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public class CurveFloatFollower : ArpegiatorFollower<float>
    {
        [SerializeField] private AnimationCurve _curve;
        
        public override float Follow(float original, float current, float arpegiated)
        {
            return arpegiated + _curve.Evaluate(current - original);
        }
    }
}