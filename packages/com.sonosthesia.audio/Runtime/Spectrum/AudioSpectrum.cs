// Modified from Audio spectrum component
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-audio-spectrum

using System;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public enum LevelType
    {
        Level,
        MeanLevel,
        PeakLevel
    }
    
    public abstract class AudioSpectrum : MonoBehaviour
    {
        public enum BandType 
        {
            FourBand,
            FourBandVisual,
            EightBand,
            TenBand,
            TwentySixBand,
            ThirtyOneBand
        };

        private static readonly float[][] _middleFrequenciesForBands = {
            new float[]{ 125.0f, 500, 1000, 2000 },
            new float[]{ 250.0f, 400, 600, 800 },
            new float[]{ 63.0f, 125, 500, 1000, 2000, 4000, 6000, 8000 },
            new float[]{ 31.5f, 63, 125, 250, 500, 1000, 2000, 4000, 8000, 16000 },
            new float[]{ 25.0f, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000 },
            new float[]{ 20.0f, 25, 31.5f, 40, 50, 63, 80, 100, 125, 160, 200, 250, 315, 400, 500, 630, 800, 1000, 1250, 1600, 2000, 2500, 3150, 4000, 5000, 6300, 8000, 10000, 12500, 16000, 20000 },
        };

        private static readonly float[] _bandwidthForBands = {
            1.414f, // 2^(1/2)
            1.260f, // 2^(1/3)
            1.414f, // 2^(1/2)
            1.414f, // 2^(1/2)
            1.122f, // 2^(1/6)
            1.122f  // 2^(1/6)
        };

        [SerializeField] private int _numberOfSamples = 1024;
        [SerializeField] private FFTWindow _fftWindow = FFTWindow.BlackmanHarris;
        [SerializeField] private BandType _bandType = BandType.TenBand;
        [SerializeField] private float _fallSpeed = 0.08f;
        [SerializeField] private float _sensitivity = 8.0f;

        private float[] _rawSpectrum;

        public float[] Levels { get; private set; }

        public float[] PeakLevels { get; private set; }

        public float[] MeanLevels { get; private set; }

        public float[] GetLevels(LevelType levelType) => levelType switch
        {
            LevelType.Level => Levels,
            LevelType.MeanLevel => MeanLevels,
            LevelType.PeakLevel => PeakLevels,
            _ => throw new ArgumentOutOfRangeException(nameof(levelType), levelType, null)
        };
        
        private void CheckBuffers ()
        {
            if (_rawSpectrum == null || _rawSpectrum.Length != _numberOfSamples) 
            {
                _rawSpectrum = new float[_numberOfSamples];
            }
            
            int bandCount = _middleFrequenciesForBands [(int)_bandType].Length;
            if (Levels == null || Levels.Length != bandCount) 
            {
                Levels = new float[bandCount];
                PeakLevels = new float[bandCount];
                MeanLevels = new float[bandCount];
            }
        }

        private int FrequencyToSpectrumIndex (float f)
        {
            int i = Mathf.FloorToInt (f / AudioSettings.outputSampleRate * 2.0f * _rawSpectrum.Length);
            return Mathf.Clamp (i, 0, _rawSpectrum.Length - 1);
        }

        protected abstract void GetSpectrumData(float[] spectrum, int channel, FFTWindow window);

        protected virtual void Awake ()
        {
            CheckBuffers ();
        }

        protected void Update ()
        {
            CheckBuffers ();

            GetSpectrumData(_rawSpectrum, 0, FFTWindow.BlackmanHarris);

            float[] middlefrequencies = _middleFrequenciesForBands [(int)_bandType];
            float bandwidth = _bandwidthForBands [(int)_bandType];
            float falldown = _fallSpeed * Time.deltaTime;
            float filter = Mathf.Exp (-_sensitivity * Time.deltaTime);

            for (var bi = 0; bi < Levels.Length; bi++) 
            {
                int imin = FrequencyToSpectrumIndex (middlefrequencies [bi] / bandwidth);
                int imax = FrequencyToSpectrumIndex (middlefrequencies [bi] * bandwidth);

                float bandMax = 0.0f;
                for (int fi = imin; fi <= imax; fi++) 
                {
                    bandMax = Mathf.Max (bandMax, _rawSpectrum [fi]);
                }

                Levels [bi] = bandMax;
                PeakLevels [bi] = Mathf.Max (PeakLevels [bi] - falldown, bandMax);
                MeanLevels [bi] = bandMax - (bandMax - MeanLevels [bi]) * filter;
            }
        }
    }
}