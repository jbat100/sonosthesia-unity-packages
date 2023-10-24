using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Sonosthesia.Arpeggiator
{
    public abstract class FrozenScheduler : Scheduler
    {
        protected class FrozenSession : ISession
        {
            public IObservable<float> Stream { get; }

            public FrozenSession(IObservable<float> stream)
            {
                Stream = stream;
            }

            public void Dispose()
            {
                
            }
        }

        private static readonly List<float> _buffer = new ();
        
        protected override ISession CreateSession(bool loop)
        {
            _buffer.Clear();
            TimeOffsets(_buffer);
            float duration = _buffer[^1];
            IObservable<float> stream = _buffer
                .Select(ProcessOffset)
                .Select(offset => Observable.Timer(TimeSpan.FromSeconds(offset)).Select(_ => offset / duration))
                .Merge();
            if (loop)
            {
                stream = stream.Repeat();
            }
            return new FrozenSession(stream);
        }
        
        // add randomization or whatever
        protected virtual float ProcessOffset(float offset)
        {
            return offset;
        }

        // note : no need for duration, it's the last value in the buffer
        protected void TimeOffsets(List<float> buffer)
        {
            InternalTimeOffsets(buffer);
        }
        
        protected abstract void InternalTimeOffsets(List<float> buffer);
    }
}