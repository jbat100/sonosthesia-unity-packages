using UnityEngine;

namespace Sonosthesia.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceSpectrum : UnityAudioSpectrum
    {
        private AudioSource _source;
        
        protected override void Awake()
        {
            base.Awake();
            _source = GetComponent<AudioSource>();
        }

        protected override bool GetSpectrumData(float[] spectrum, int channel, FFTWindow window)
        {
            _source.GetSpectrumData(spectrum, channel, window);
            return true;
        }
    }
}