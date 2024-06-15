using UnityEngine;

namespace Sonosthesia.Audio
{
    public class AudioListenerSpectrum : UnityAudioSpectrum
    {
        protected override bool GetSpectrumData(float[] spectrum, int channel, FFTWindow window)
        {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            return true;
        }
    }
}