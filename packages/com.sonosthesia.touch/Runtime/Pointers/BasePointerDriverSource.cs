using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Base class allows non templated affordances to access event streams
    /// </summary>
    public class BasePointerDriverSource : MonoBehaviour
    {
        // used for affordances
        public readonly struct SourceEvent
        {
            public readonly Guid Id;
            public readonly PointerEventData Data;

            public SourceEvent(Guid id, PointerEventData data)
            {
                Id = id;
                Data = data;
            }
        }
        
        private readonly ReactiveCollection<Guid> _ongoingEvents = new();
        public IReadOnlyReactiveCollection<Guid> OngoingEvents => _ongoingEvents;

        private readonly Subject<IObservable<SourceEvent>> _sourceStreamSubject = new();
        public IObservable<IObservable<SourceEvent>> SourceStreamObservable => _sourceStreamSubject.AsObservable();

        protected void Pipe(IObservable<SourceEvent> observable)
        {
            _sourceStreamSubject.OnNext(observable);
        }

        protected void RegisterEvent(Guid eventId) => _ongoingEvents.Add(eventId);
        protected void UnregisterEvent(Guid eventId) => _ongoingEvents.Remove(eventId);
    }
}