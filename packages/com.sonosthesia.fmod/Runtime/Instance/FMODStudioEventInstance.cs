using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODStudioEventInstance : FMODInstance
    {
        [SerializeField] private StudioEventEmitter _emitter;

        private bool _dirty;
        
        public override void Restart()
        {
            UnityEngine.Debug.LogWarning($"Before {nameof(Restart)} instance is {_emitter.EventInstance.handle}");
            _emitter.Play();
            UnityEngine.Debug.LogWarning($"After {nameof(Restart)} instance is {_emitter.EventInstance.handle}");
            _dirty = true;
        }

        public override void Stop()
        {
            _emitter.Stop();
            _dirty = true;
        }
        
        protected virtual void Update()
        {
            // A bit nasty but StudioEventEmitter has no way of knowing from outside 
            // if a new event instance has been created for sure, and previous one
            // shot may be still playing
            
            // Even checking handle doesn't work it stays the same throughout plays
            
            if (_dirty || _emitter.EventInstance.handle != EventInstance.handle)
            {
                _dirty = false;
                EventInstance = _emitter.EventInstance;
            }
        }
    }
}