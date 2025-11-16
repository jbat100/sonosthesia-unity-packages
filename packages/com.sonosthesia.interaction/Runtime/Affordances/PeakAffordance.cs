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
                
                _magnitude = configuration.Magnitude.StartSession(e);
                _duration = configuration.Duration.StartSession(e);

                _speed = configuration.Speed.StartSession(e);
                _chaos = configuration.Chaos.StartSession(e);

                _schedulerSession = configuration.Scheduler.MakeSession(_speed.Evaluate(), _chaos.Evaluate());
                
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _schedulerSession.Speed = _speed.Evaluate();
                    _schedulerSession.Chaos = _chaos.Evaluate();
                });

                _eventSubscription = _schedulerSession.Stream.Subscribe(s =>
                {
                    float duration = _duration.Evaluate().NormalizedRandomization(configuration.Randomization);
                    float magnitude = _magnitude.Evaluate().NormalizedRandomization(configuration.Randomization);
                    Affordance._target.Broadcast(new Peak(duration, magnitude, 0));
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