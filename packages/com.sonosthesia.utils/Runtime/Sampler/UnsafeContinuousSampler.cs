using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    // note : for improved performance expects array to be non empty and constant sorted in ascending time
    // this is not enforced, if this is not enforced by client, behaviour is undefined
    // note : this utility offers a benefit in performance but only if the calls to TryGet are made with
    // increasing time values
    
    public class UnsafeContinuousSampler<T> where T : struct, ITimed, ILerpable<T>
    {
        private readonly T[] _samples;
        private readonly SamplerInfo<T> _info;

        private int _currentIndex = 0;

        public UnsafeContinuousSampler(T[] samples)
        {
            _samples = samples;
            _currentIndex = 0;
            _info = new SamplerInfo<T>(samples);
        }

        public bool TryGet(float time, out T sample)
        {
            if (_info.Count == 0)
            {
                _currentIndex = 0;
                sample = default;
                return false;
            }

            if (time <= _info.StartTime)
            {
                sample = _info.StartValue;
                return true;
            }

            if (time >= _info.EndTime)
            {
                sample = _info.EndValue;
                return true;
            }

            _currentIndex = math.clamp(_currentIndex, 0, _info.Count - 1);

            // this is a reset on scan optimization failure, client probably called TryGet with decreasing time value
            T currentSample = _samples[_currentIndex];
            if (time < currentSample.GetTime())
            {
                _currentIndex = 0;
            }

            for (int i = _currentIndex; i <= _info.Count - 1; i++)
            {
                T lower = _samples[i];
                T upper = _samples[i + 1];
                float lowerTime = lower.GetTime();
                float upperTime = upper.GetTime();
                if (time >= lower.GetTime() && time <= upper.GetTime())
                {
                    _currentIndex = i;
                    sample = lower.Lerp(upper, MathUtils.InverseLerp(lowerTime, upperTime, time));
                    return true;
                }
            }

            _currentIndex = 0;
            sample = default;
            return false;
        }
    }
}