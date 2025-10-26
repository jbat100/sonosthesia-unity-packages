using System;
using Sonosthesia.Scheduler;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class SchedulerAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private Signal.Signal<SchedulerEvent> _target;

        [SerializeField] private InterfaceReference<ISchedulerConfiguration<TEvent>> _configuration;

        private class Controller : AffordanceController<TEvent, SchedulerAffordance<TEvent>>, IDisposable
        {
            private IInteractiveEnvelopeSession<TEvent> _speedSession;
            private IInteractiveEnvelopeSession<TEvent> _chaosSession;
            private ISchedulerSession _schedulerSession;
            private IDisposable _updateSubscription;
            private IDisposable _eventSubscription;
            
            public Controller(Guid eventId, SchedulerAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                ISchedulerConfiguration<TEvent> configuration = Affordance._configuration.Value;
                Signal.Signal<SchedulerEvent> target = Affordance._target;

                _speedSession = configuration.Speed.StartSession(e);
                _chaosSession = configuration.Chaos.StartSession(e);

                _schedulerSession =  configuration.Scheduler.MakeSession(_speedSession.Update(), _chaosSession.Update());
                
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _schedulerSession.Speed = _speedSession.Update();
                    _schedulerSession.Chaos = _chaosSession.Update();
                });

                _eventSubscription = _schedulerSession.Stream.Subscribe(s => target.Broadcast(s));
            }

            protected override void Update(TEvent e)
            {
                base.Update(e);
                _speedSession.Update(e);
                _chaosSession.Update(e);
            }

            protected override void Teardown(TEvent e)
            {
                base.Teardown(e);
                _speedSession.End(e, out float _);
                _chaosSession.End(e, out float _);
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