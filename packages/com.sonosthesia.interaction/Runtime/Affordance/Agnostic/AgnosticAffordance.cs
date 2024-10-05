using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    /// <summary>
    /// Affordance for a given event stream container, agnostic of value 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TAffordance"></typeparam>
    public class AgnosticAffordance<TEvent, TContainer, TAffordance> : MonoBehaviour 
        where TEvent : struct 
        where TContainer : MonoBehaviour, IEventStreamContainer<TEvent>
        where TAffordance : AgnosticAffordance<TEvent, TContainer, TAffordance>
    {
        [SerializeField] private bool _log;
        protected bool Log => _log;
        
        // TODO : relink in editor
        [SerializeField] private TContainer _container;

        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void OnEventCountChanged(int count)
        {
            
        }

        protected virtual IObserver<TEvent> MakeController(Guid id) => null;
        
        protected virtual void HandleStream(Guid id, IObservable<TEvent> stream)
        {
            
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Add(_container.EventStreamNode.Values.ObserveCountChanged().Subscribe(OnEventCountChanged));
            _subscriptions.Add(_container.EventStreamNode.StreamObservable.Subscribe(pair =>
            {
                Guid id = pair.Key;
                if (_log)
                {
                    Debug.Log($"{this} handling new stream {id}");
                }
                IObservable<TEvent> stream = pair.Value.TakeUntilDisable(this);
                IObserver<TEvent> controller = MakeController(id);
                if (controller != null)
                {
                    stream.Subscribe(controller);
                }
                HandleStream(pair.Key, stream);
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
        
        // For convenience, subclasses can inherit Controller
        
        protected abstract class Controller : IObserver<TEvent>
        {
            private readonly TAffordance _affordance;
            protected TAffordance Affordance => _affordance;
            
            private readonly Guid _eventId;
            protected Guid EventId => _eventId;
            
            private bool _initialized;
            private TEvent _latest;

            public Controller(Guid eventId, TAffordance affordance)
            {
                _eventId = eventId;
                _affordance = affordance;
            }
 
            protected virtual void Setup(TEvent e)
            {
                if (_affordance._log)
                {
                    Debug.Log($"{_affordance} controller {nameof(Setup)} {e}");
                }
            }

            protected virtual void Update(TEvent e)
            {
                if (_affordance._log)
                {
                    // Debug.Log($"{_affordance} controller {nameof(Update)} {e}");
                }
            }

            protected virtual void Teardown(TEvent e)
            {
                if (_affordance._log)
                {
                    Debug.Log($"{_affordance} controller {nameof(Teardown)} {e}");
                }
            }

            public void OnNext(TEvent e)
            {
                _latest = e;
                
                if (!_initialized)
                {
                    Setup(e);
                    _initialized = true;
                }
                else
                {
                    Update(e);   
                }
            }
            
            public void OnCompleted()
            {
                Teardown(_latest);
            }

            public void OnError(Exception error)
            {
                Teardown(_latest);
            }
        }
    }
}