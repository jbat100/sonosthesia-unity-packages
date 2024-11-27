using UnityEngine;

namespace Sonosthesia.Scheduler
{
    public class DynamicScheduler : LoopingScheduler
    {
        private class DynamicSession : AbstractSchedulerSession
        {
            private readonly float _duration;
            private readonly bool _repeat;
            private readonly float[] _offsets;
            
            private float[] _randomisation;
            private float _loopTime;
            private float _time;
            private int _count;

            public DynamicSession(float speed, float chaos, float[] offsets, float duration, bool repeat) : base(speed, chaos)
            {
                _offsets = offsets;
                _duration = duration;
                _repeat = repeat;
                _loopTime = 0;
                _time = 0;
                _count = 0;
                Randomize();
            }

            protected override void Update()
            {
                float previousLoopTime = _loopTime;
                float deltaTime = Time.deltaTime * Speed;
                _loopTime += deltaTime;
                _time += deltaTime;
                if (previousLoopTime > _duration)
                {
                    if (!_repeat)
                    {
                        Dispose();
                        return;
                    }
                    _loopTime %= _duration;
                    Randomize();
                }
                for (int i = 0; i < _offsets.Length; i++)
                {
                    float value = _offsets[i] + _randomisation[i] * Chaos;
                    if (value >= previousLoopTime && value <= _loopTime)
                    {
                        Push(new SchedulerEvent(_count, _time, _loopTime / _duration));
                        _count++;
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
        }

        protected override ISchedulerSession CreateSession(float speed, float chaos, bool repeat, SchedulerLoop loop)
        {
            return new DynamicSession(speed, chaos, loop.Offsets, loop.Duration, repeat);
        }
    }
}