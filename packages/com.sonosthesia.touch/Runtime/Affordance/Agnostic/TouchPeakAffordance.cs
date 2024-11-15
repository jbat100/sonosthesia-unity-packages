using System;
using Sonosthesia.Interaction;
using Sonosthesia.Scheduler;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchPeakAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private Signal<Peak> _target;

        [SerializeField] private TouchPeakConfiguration _peakConfiguration;
        
        [SerializeField] private TouchSchedulerConfiguration _schedulerConfiguration;

        private class Controller : AffordanceController<TouchEvent, TouchPeakAffordance>, IDisposable
        {
            private ITouchEnvelopeSession _magnitude;
            private ITouchEnvelopeSession _duration;
            
            private ITouchEnvelopeSession _speed;
            private ITouchEnvelopeSession _chaos;
            
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
                TouchSchedulerConfiguration schedulerConfiguration = Affordance._schedulerConfiguration;
                Signal<Peak> target = Affordance._target;

                _magnitude = peakConfiguration.Magnitude.SetupSession(e);
                _duration = peakConfiguration.Duration.SetupSession(e);

                _speed = schedulerConfiguration.Speed.SetupSession(e);
                _chaos = schedulerConfiguration.Chaos.SetupSession(e);

                _schedulerSession = schedulerConfiguration.Scheduler.CreateSession(_speed.Update(), _chaos.Update());
                
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
                _magnitude.UpdateTouch(e);
                _duration.UpdateTouch(e);
                _speed.UpdateTouch(e);
                _chaos.UpdateTouch(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                _magnitude.EndTouch(e, out float _);
                _duration.EndTouch(e, out float _);
                _speed.EndTouch(e, out float _);
                _chaos.EndTouch(e, out float _);
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