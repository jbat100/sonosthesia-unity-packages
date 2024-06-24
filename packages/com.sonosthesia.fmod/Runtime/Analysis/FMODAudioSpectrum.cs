using Sonosthesia.Audio;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODAudioSpectrum : AudioSpectrum
    {
        [SerializeField] private FMODFFT _instanceFFT;

        [SerializeField] private FMODSampleRate _sampleRate;

        protected override void Awake()
        {
            base.Awake();
            if (NumberOfSamples != _instanceFFT.NumberOfSamples)
            {
                UnityEngine.Debug.LogError($"{nameof(NumberOfSamples)} mismatch");
            }
        }

        protected override float OutputSampleRate => _sampleRate.OutputSampleRate;
        
        protected override bool GetSpectrumData(float[] spectrum, int channel)
        {
            return _instanceFFT && _instanceFFT.GetSpectrumData(spectrum, channel);
        }
    }
}