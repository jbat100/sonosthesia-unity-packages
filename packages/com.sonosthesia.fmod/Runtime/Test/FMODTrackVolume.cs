using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODTrackVolume : MonoBehaviour
    {
        public EventReference fmodEventPath;
        
        private EventInstance eventInstance;

        protected virtual void Start()
        {
            // Create an instance of the event
            eventInstance = RuntimeManager.CreateInstance(fmodEventPath);
        
            // Start the event
            eventInstance.start();
        }

        public void SetTrackVolume(string parameterName, float volume)
        {
            // Get the parameter instance
            PARAMETER_ID parameterId;
            eventInstance.getDescription(out EventDescription eventDescription);
            eventDescription.getParameterDescriptionByName(parameterName, out PARAMETER_DESCRIPTION parameterDescription);
            parameterId = parameterDescription.id;

            // Set the volume of the track
            eventInstance.setParameterByID(parameterId, volume);
        }

        void OnDestroy()
        {
            // Stop and release the event instance when the object is destroyed
            eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }    
}


