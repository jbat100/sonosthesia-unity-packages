using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class DynamicScheduler : Scheduler
    {
        [SerializeField] private SchedulerConfiguration _configuration;

        [SerializeField] private bool _repeat;
        
        [SerializeField] private float _chaos;
        
        [SerializeField] private float _warp = 1f;
        
        private class DynamicSession : ISession
        {
            private readonly float _duration;
            private readonly bool _loop;
            private readonly bool _repeat;
            private readonly Func<float> _chaos;
            private readonly Func<float> _warp;
            private readonly float[] _offsets;
            
            private float[] _randomisation;
            private float _time;
            private int _index;
            private IDisposable _updateSubscription;
            private Subject<float> _stream = new ();
            
            public IObservable<float> Stream => _stream.AsObservable();

            public DynamicSession(float[] offsets, float duration, bool loop, bool repeat, Func<float> chaos, Func<float> warp)
            {
                _offsets = offsets;
                _duration = duration;
                _loop = loop;
                _repeat = repeat;
                _chaos = chaos;
                _warp = warp;
                Randomize();
                _time = 0;
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ => Update());
            }

            private void Update()
            {
                float previousTime = _time;
                _time += Time.deltaTime * _warp();
                if (previousTime > _duration)
                {
                    if (!_loop)
                    {
                        Dispose();
                        return;
                    }
                    _time %= _duration;
                    if (!_repeat)
                    {
                        Randomize();
                    }
                }
                float chaos = _chaos();
                for (int i = 0; i < _offsets.Length; i++)
                {
                    float value = _offsets[i] + _randomisation[i] * chaos;
                    if (value >= previousTime && value <= _time)
                    {
                        _stream.OnNext(_time / _duration);
                    }
                }
            }

            private void Randomize()
            {
                _randomisation = new float[_offsets.Length];
                for (int i = 0; i < _offsets.Length; i++)
                {
                    _randomisation[i] = UnityEngine.Random.Range(-1, 1);
                }
            }
            
            public void Dispose()
            {
                _updateSubscription?.Dispose();
                _updateSubscription = null;
                _stream?.OnCompleted();
                _stream?.Dispose();
                _stream = null;
            }
        }

        protected override ISession CreateSession(bool loop)
        {
            return new DynamicSession(_configuration.Offsets, 
                _configuration.Duration, loop, _repeat, 
                () => _chaos, () => _warp);
        }
    }
}