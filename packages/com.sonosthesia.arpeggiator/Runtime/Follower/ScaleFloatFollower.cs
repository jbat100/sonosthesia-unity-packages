using UnityEngine;

namespace Sonosthesia.Arpeggiator
{
    public class ScaleFloatFollower : ArpegiatorFollower<float>
    {
        [SerializeField] private float _scale = 1f;
        
        public override float Follow(float original, float current, float arpegiated)
        {
            return arpegiated + (current - original) * _scale;
        }
    }
}