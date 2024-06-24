using FMOD;
using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODSampleRate : MonoBehaviour
    {
        private const float DEFAULT_SAMPLE_RATE = 44000;
        
        private float _sampleRate;
        private bool _setupDone;

        public float OutputSampleRate
        {
            get
            {
                if (!_setupDone)
                {
                    _setupDone = TrySetup();
                }
                
                return _setupDone ? _sampleRate : DEFAULT_SAMPLE_RATE;
            }
        }

        private bool TrySetup()
        {
            RESULT result = RuntimeManager.CoreSystem.getSoftwareFormat(out int sampleRate, out _, out _);
            UnityEngine.Debug.LogWarning($"getSoftwareFormat {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            _sampleRate = sampleRate;
            return true;
        }
    }
}