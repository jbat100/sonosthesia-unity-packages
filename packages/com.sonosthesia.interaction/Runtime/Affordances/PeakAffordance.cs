using System;
using Sonosthesia.Scheduler;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class PeakAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private Signal.Signal<Peak> _target;

        [SerializeField] private InterfaceReference<IPeakConfiguration<TEvent>> _configuration;
        
        private class Controller : AffordanceController<TEvent, PeakAffordance<TEvent>>, IDisposable
        {
            private IInteractiveEnvelopeSession<TEvent> _magnitude;
            private IInteractiveEnvelopeSession<TEvent> _duration;
            
            private IInteractiveEnvelopeSession<TEvent> _speed;
            private IInteractiveEnvelopeSession<TEvent> _chaos;
            
            private ISchedulerSession _schedulerSession;
            
            private IDisposable _updateSubscription;
            private IDisposable _eventSubscription;
            
            public Controller(Guid eventId, PeakAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                IPeakConfiguration<TEvent> configuration = Affordance._configuration.Value;
                Signal.Signal<Peak> target = Affordance._target;

                _magnitude = configuration.Magnitude.StartSession(e);
                _duration = configuration.Duration.StartSession(e);

                _speed = configuration.Speed.StartSession(e);
                _chaos = configuration.Chaos.StartSession(e);

                _schedulerSession = configuration.Scheduler.MakeSession(_speed.Update(), _chaos.Update());
                
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _magnitude.Update();
                    _duration.Update();
                    _schedulerSession.Speed = _speed.Update();
                    _schedulerSession.Chaos = _chaos.Update();
                });

                _eventSubscription = _schedulerSession.Stream.Subscribe(s =>
                {
                    float duration = _duration.Update().NormalizedRandomization(configuration.Randomization);
                    float magnitude = _magnitude.Update().NormalizedRandomization(configuration.Randomization);
                    target.Broadcast(new Peak(duration, magnitude, 0));
                });
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                _magnitude.Update(e);
                _duration.Update(e);
                _speed.Update(e);
                _chaos.Update(e);
            }

            protected override void Teardown(TEvent e)
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

        protected override IObserver<TEvent> MakeController(Guid id) => new Controller(id, this);
    }
}