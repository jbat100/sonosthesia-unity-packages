using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    // note : for improved performance expects array to be non empty and constant sorted in ascending time
    // this is not enforced, if this is not enforced by client, behaviour is undefined
    // note : this utility offers a benefit in performance but only if the calls to TryGet are made with
    // increasing time values
    // note : when a call to TryGet succeeds (returns true), the sample is consumed, further calls to TryGet 
    // with increasing time arguments will not succeeds until the time of the next sample is reached
    
    public class UnsafeDiscreteSampler<T> where T : ITimed
    {
        private readonly T[] _samples;
        private readonly SamplerInfo<T> _info;

        private int _currentIndex;
        private float? _currentTime;

        public UnsafeDiscreteSampler(T[] samples)
        {
            _samples = samples;
            _info = new SamplerInfo<T>(samples);
            Reset();
        }

        public void Reset()
        {
            _currentIndex = 0;
            _currentTime = null;
        }
        
        public void TryGet(float time, List<T> results)
        {
            results.Clear();

            if (_info.Count == 0)
            {
                return;
            }
            
            if (_currentTime == null)
            {
                _currentTime = time;
                return;
            }

            if (time < _currentTime.Value)
            {
                _currentIndex = 0;
                _currentTime = time;
                return;
            }

            while (_currentIndex < _info.Count)
            {
                T sample = _samples[_currentIndex];
                float sampleTime = sample.GetTime();
                if (sampleTime > time)
                {
                    return;
                }
                results.Add(sample);
                _currentTime = sampleTime;
                _currentIndex++;
            }
        }
    }
}