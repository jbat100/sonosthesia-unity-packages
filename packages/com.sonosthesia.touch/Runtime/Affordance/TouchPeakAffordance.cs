using System;
using Sonosthesia.Interaction;
using Sonosthesia.Scheduler;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchPeakAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private Signal<Peak> _target;

        [SerializeField] private TouchPeakConfiguration _peakConfiguration;
        
        private class Controller : AffordanceController<TouchEvent, TouchPeakAffordance>, IDisposable
        {
            private IInteractiveEnvelopeSession<TouchEvent> _magnitude;
            private IInteractiveEnvelopeSession<TouchEvent> _duration;
            
            private IInteractiveEnvelopeSession<TouchEvent> _speed;
            private IInteractiveEnvelopeSession<TouchEvent> _chaos;
            
            private ISchedulerSession _schedulerSession;
            
            private IDisposable _updateSubscription;
            private IDisposable _eventSubscription;
            
            public Controller(Guid eventId, TouchPeakAffordance affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchPeakConfiguration peakConfiguration = Affordance._peakConfiguration;
                Signal<Peak> target = Affordance._target;

                _magnitude = peakConfiguration.Magnitude.StartSession(e);
                _duration = peakConfiguration.Duration.StartSession(e);

                _speed = peakConfiguration.Speed.StartSession(e);
                _chaos = peakConfiguration.Chaos.StartSession(e);

                _schedulerSession = peakConfiguration.Scheduler.MakeSession(_speed.Update(), _chaos.Update());
                
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _magnitude.Update();
                    _duration.Update();
                    _schedulerSession.Speed = _speed.Update();
                    _schedulerSession.Chaos = _chaos.Update();
                });

                _eventSubscription = _schedulerSession.Stream.Subscribe(s =>
                {
                    float duration = _duration.Update().NormalizedRandomization(peakConfiguration.Randomization);
                    float magnitude = _magnitude.Update().NormalizedRandomization(peakConfiguration.Randomization);
                    target.Broadcast(new Peak(duration, magnitude, 0));
                });
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                _magnitude.Update(e);
                _duration.Update(e);
                _speed.Update(e);
                _chaos.Update(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                _magnitude.End(e, out float _);
                _duration.End(e, out float _);
                _speed.End(e, out float _);
                _chaos.End(e, out float _);
                Dispose();
            }

            public void Dispose()
            {
                _schedulerSession?.Dispose();
                _updateSubscription?.Dispose();
                _eventSubscription?.Dispose();
            }
        }

        protected override IObserver<TouchEvent> MakeController(Guid id) => new Controller(id, this);
    }
}