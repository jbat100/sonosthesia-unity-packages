using UnityEngine;

namespace Sonosthesia.Audio
{
    public abstract class UnityAudioSpectrum : AudioSpectrum
    {
        [SerializeField] private FFTWindow _fftWindow = FFTWindow.BlackmanHarris;
        
        protected abstract bool GetSpectrumData(float[] spectrum, int channel, FFTWindow window);

        protected override bool GetSpectrumData(float[] spectrum, int channel) => GetSpectrumData(spectrum, channel, _fftWindow);

        protected override float OutputSampleRate => AudioSettings.outputSampleRate;
    }
}