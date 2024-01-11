using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// ITriggerData plays the same role as PointerEventData
    /// </summary>

    public interface ITriggerData
    {
        Collider Collider { get; }
        bool Colliding { get; }
        BaseTriggerChannelSource Source { get; }
        BaseTriggerActor Actor { get; }
    }
    
    // used for affordances
    public readonly struct TriggerSourceEvent
    {
        public readonly Guid Id;
        public readonly ITriggerData TriggerData;

        public TriggerSourceEvent(Guid id, ITriggerData triggerData)
        {
            Id = id;
            TriggerData = triggerData;
        }
    }
    
    public class BaseTriggerChannelSource : MonoBehaviour
    {
        private readonly ReactiveCollection<Guid> _ongoingEvents = new();
        public IReadOnlyReactiveCollection<Guid> OngoingEvents => _ongoingEvents;

        private readonly Subject<IObservable<TriggerSourceEvent>> _sourceStreamSubject = new();
        public IObservable<IObservable<TriggerSourceEvent>> SourceStreamObservable => _sourceStreamSubject.AsObservable();
        
        protected void Pipe(IObservable<TriggerSourceEvent> observable)
        {
            _sourceStreamSubject.OnNext(observable);
        }

        protected void RegisterEvent(Guid eventId) => _ongoingEvents.Add(eventId);
        protected void UnregisterEvent(Guid eventId) => _ongoingEvents.Remove(eventId);
    }
}