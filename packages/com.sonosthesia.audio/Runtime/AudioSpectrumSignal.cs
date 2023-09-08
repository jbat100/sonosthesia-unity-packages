using Sonosthesia.Flow;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Audio
{
    public class AudioSpectrumSignal : Signal<float>
    {
        [SerializeField] private AudioSpectrum _spectrum;

        [SerializeField] private int[] _bins;

        [SerializeField] private LevelType _levelType;
        
        [SerializeField] private FloatWarpSettings _warp;

        [SerializeField] private FloatOneEuroFilterSettings _oneEuroFilter;

        [SerializeField] private FloatSoftLandingSettings _softLanding;

        private ProcessorChain<float> _chain = new();

        protected void Awake()
        {
            if (!_spectrum)
            {
                _spectrum = GetComponentInParent<AudioSpectrum>();
            }

            _chain = new ProcessorChain<float>(
                new FloatWarpProcessor(_warp),
                new FloatOneEuroFilterProcessor(_oneEuroFilter),
                new FloatSoftLandingProcessor(_softLanding));
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

            result = _chain.Process(result, Time.time);
            Broadcast(result);
        }

        protected void OnEnable()
        {
            _chain.Reset();
        }
    }
}