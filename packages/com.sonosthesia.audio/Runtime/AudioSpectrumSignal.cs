using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public class AudioSpectrumSignal : Signal<float>
    {
        [SerializeField] private AudioSpectrum _spectrum;

        [SerializeField] private int[] _bins;

        [SerializeField] private LevelType _levelType;

        [SerializeField] private bool _db;

        [SerializeField] private float _scale = 1f;

        [SerializeField] private float _offset;
        
        protected void Awake()
        {
            if (!_spectrum)
            {
                _spectrum = GetComponentInParent<AudioSpectrum>();
            }
        }

        protected void Update()
        {
            float result = 0f;
            float[] data = _spectrum.GetLevels(_levelType);
            for (int i = 0; i < _bins.Length; i++)
            {
                int bin = _bins[i];
                if (data.Length <= bin)
                {
                    Debug.LogWarning($"Bin index {bin} is out of bounds {data.Length}");
                    continue;
                }

                result += data[bin];
            }
            if (_db)
            {
                result = 10f * Mathf.Log10(result + 1f);
            }
            result = (result * _scale) + _offset;
            Broadcast(result);
        }
        
        
    }
}