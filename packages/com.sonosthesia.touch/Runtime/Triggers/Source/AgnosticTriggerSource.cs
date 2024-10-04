using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AgnosticTriggerSource : TriggerSource
    {
        private readonly Dictionary<Guid, BehaviorSubject<TriggerEvent>> _eventSubjects = new();

        protected override bool ConfigureStream(Guid eventId, ITriggerData triggerData)
        {
            TriggerEvent sourceEvent = new TriggerEvent(eventId, triggerData, Time.time);
            BehaviorSubject<TriggerEvent> eventSubject = new BehaviorSubject<TriggerEvent>(sourceEvent);
            _eventSubjects[sourceEvent.Id] = eventSubject;
            IObservable<TriggerEvent> sourceObservable = eventSubject.AsObservable();
            EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            // push the stream to the actor
            if (sourceEvent.TriggerData.Actor)
            {
                sourceEvent.TriggerData.Actor.EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            }
            return true;
        }

        protected override void UpdateStream(Guid eventId, ITriggerData triggerData)
        {
            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerEvent> subject))
            {
                return;
            }
            subject.OnNext(subject.Value);
        }

        protected override void CleanupStream(Guid eventId, ITriggerData triggerData)
        {
            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<TriggerEvent> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _eventSubjects.Remove(eventId);
        }
    }
}