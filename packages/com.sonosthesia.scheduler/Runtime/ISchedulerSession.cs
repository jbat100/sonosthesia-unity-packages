using System;
using Sonosthesia.Utils;
using UniRx;

namespace Sonosthesia.Scheduler
{
    public readonly struct SchedulerEvent
    {
        public readonly int count;
        public readonly float time;
        public readonly float position;

        public SchedulerEvent(int count, float time, float position)
        {
            this.count = count;
            this.time = time;
            this.position = position;
        }

        public override string ToString()
        {
            return $"{nameof(SchedulerEvent)} {nameof(count)} {count} {nameof(time)} {time} {nameof(position)} {position}";
        }
    }
    
    public interface ISchedulerSession : IDisposable
    {
        // Thought about start stop but that invalidates Stream completion 
            
        /// <summary>
        /// Value is the normalized position (which can loop if specified)
        /// </summary>
        IObservable<SchedulerEvent> Stream { get; }
        
        float Speed { get; set; }
        
        float Chaos { get; set; }
    }

    public abstract class AbstractSchedulerSession : ISchedulerSession
    {
        public float Speed { get; set; } = 1f;
        public float Chaos { get; set; } = 0f;

        
        private Subject<SchedulerEvent> _subject = new();
        private readonly IDisposable _updateSubscription;
        
        public IObservable<SchedulerEvent> Stream => _subject.AsObservable();

        public AbstractSchedulerSession(float speed, float chaos)
        {
            Speed = speed;
            Chaos = chaos;
            _updateSubscription = Observable.EveryUpdate().Subscribe(_ => Update());
        }

        protected abstract void Update();

        protected void Push(SchedulerEvent e) => _subject.OnNext(e);

        public void Dispose()
        {
            _updateSubscription?.Dispose();
            RxUtils.Cleanup(ref _subject);
        }
    }
}