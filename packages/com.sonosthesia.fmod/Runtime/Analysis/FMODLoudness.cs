using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.FMOD
{
    public abstract class FMODLoudness : FMODProcessor
    {
        [SerializeField] private LoudnessSelector _selector = LoudnessSelector.Momentary;
        
        [SerializeField] private Signal<float> _target;
        
        public override void Process()
        {
            if (!_target)
            {
                return;
            }
            
            if (TryGetLoudness(_selector, out float loudness))
            {
                _target.Broadcast(loudness);
            }
        }

        protected abstract bool TryGetLoudness(LoudnessSelector selector, out float loudness);
    }
}