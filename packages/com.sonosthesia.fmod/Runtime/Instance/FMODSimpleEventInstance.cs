using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    
    public class FMODSimpleEventInstance : FMODInstance
    {
        [SerializeField] private EventReference _fmodEventPath;

        [SerializeField] private bool _autoStart;
        
        [SerializeField, Range(0, 1)] private float _volume;

        private EventInstance _eventInstance;

        protected virtual void Start()
        {
            if (_autoStart)
            {
                Restart();
            }
        }

        protected virtual void Update()
        {
            _eventInstance.setVolume(_volume);
        }

        protected virtual void OnDestroy()
        {
            // Stop and release the event instance when the object is destroyed
            _eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
            _eventInstance.release();
            EventInstance = default;
        }

        public override void Stop()
        {
            if (_eventInstance.hasHandle())
            {
                // Stop and release the event instance when the object is destroyed
                _eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
                _eventInstance.release();
                EventInstance = default;
            }
        }
        
        public override void Restart()
        {
            Stop();
            
            // Create an instance of the event
            _eventInstance = RuntimeManager.CreateInstance(_fmodEventPath);
            _eventInstance.getVolume(out _volume);
            // Start the event
            _eventInstance.start();
            EventInstance = _eventInstance;
        }
    }    
}


