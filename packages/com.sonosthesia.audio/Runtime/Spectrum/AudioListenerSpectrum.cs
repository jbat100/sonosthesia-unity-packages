using UnityEngine;

namespace Sonosthesia.Audio
{
    public class AudioListenerSpectrum : AudioSpectrum
    {
        protected override void GetSpectrumData(float[] spectrum, int channel, FFTWindow window)
        {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        }
    }
}