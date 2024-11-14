using System;
using Sonosthesia.Interaction;
using Sonosthesia.Scheduler;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchSchedulerAffordance : AbstractAffordance<TouchEvent>
    {
        [SerializeField] private Signal<SchedulerEvent> _target;

        [SerializeField] private TouchSchedulerConfiguration _configuration;

        private class Controller : AffordanceController<TouchEvent, TouchSchedulerAffordance>, IDisposable
        {
            private ITouchEnvelopeSession _speedSession;
            private ITouchEnvelopeSession _chaosSession;
            private ISchedulerSession _schedulerSession;
            private IDisposable _updateSubscription;
            private IDisposable _eventSubscription;
            
            public Controller(Guid eventId, TouchSchedulerAffordance affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TouchEvent e)
            {
                base.Setup(e);

                TouchSchedulerConfiguration configuration = Affordance._configuration;
                Signal<SchedulerEvent> target = Affordance._target;

                _speedSession = configuration.Speed.SetupSession(e);
                _chaosSession = configuration.Chaos.SetupSession(e);

                _schedulerSession = configuration.Scheduler.CreateSession(_speedSession.Update(), _chaosSession.Update());
                
                _updateSubscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _schedulerSession.Speed = _speedSession.Update();
                    _schedulerSession.Chaos = _chaosSession.Update();
                });

                _eventSubscription = _schedulerSession.Stream.Subscribe(s => target.Broadcast(s));
            }

            protected override void Update(TouchEvent e)
            {
                base.Update(e);
                _speedSession.UpdateTouch(e);
                _chaosSession.UpdateTouch(e);
            }

            protected override void Teardown(TouchEvent e)
            {
                base.Teardown(e);
                _speedSession.EndTouch(e, out float _);
                _chaosSession.EndTouch(e, out float _);
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