using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    // Note : current usage forecast is for MPENote bend restricted to semitones, like a soft snap
    // - using move toward with a max velocity is the simplest 
    // - using ease can create a more natural looking transition but poor handling of switch during ongoing transition
    // - using trajectory can help with smooth position and velocity change but gives less control on transition
    
    // Note this can be applied as:
    // - postprocessor at the TouchMPENoteAffordance level which would allow visual guides
    // - could be a special case processor on TouchMPENoteConfiguration, separate from FloatTouchExtractorSettings
    // - processing at the MPENoteChannel level which would ease controller switch but complicate visual guides
    
    // Think about having an actor modifier (bool) in addition to actor modulator ([0, 1]) one of which could be snap

    // TODO : this is tricky as we don't want a bin switch during a transition to start another transition
    // it could be that using Trajectory instead of Ease could solve this but it would remove any
    // control Ease gives for adjacent bin switch.
    
    public class FloatBinsSession : IDisposable
    {
        private readonly FloatBinsSettings _bins;
        private IDisposable _subscription;

        private readonly struct Sample
        {
            public readonly float time;
            public readonly float value;

            public Sample(float value)
            {
                time = Time.time;
                this.value = value;
            }
        }

        private Sample _target;
        public float Target
        {
            get => _target.value;
            set
            {
                Sample previous = _target;
                _target = new Sample(value);
                
                // TODO : detect switch
            }
        }

        private float _current;

        public FloatBinsSession(FloatBinsSettings bins)
        {
            _bins = bins;
            _subscription = Observable.EveryUpdate().Subscribe(_ =>
            {

            });
        }
        
        public void Dispose()
        {
        }
    }
}