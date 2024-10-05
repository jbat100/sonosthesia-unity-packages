using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AgnosticTouchSource : TouchSource
    {
        private readonly Dictionary<Guid, BehaviorSubject<TouchEvent>> _eventSubjects = new();

        protected override bool ConfigureStream(Guid eventId, ITouchData touchData)
        {
            TouchEvent sourceEvent = new TouchEvent(eventId, touchData, Time.time);
            BehaviorSubject<TouchEvent> eventSubject = new BehaviorSubject<TouchEvent>(sourceEvent);
            _eventSubjects[sourceEvent.Id] = eventSubject;
            IObservable<TouchEvent> sourceObservable = eventSubject.AsObservable();
            EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            // push the stream to the actor
            if (sourceEvent.TouchData.Actor)
            {
                sourceEvent.TouchData.Actor.EventStreamNode.Push(sourceEvent.Id, sourceObservable);
            }
            return true;
        }

        protected override void UpdateStream(Guid eventId, ITouchData touchData)
        {
            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<TouchEvent> subject))
            {
                return;
            }
            subject.OnNext(subject.Value);
        }

        protected override void CleanupStream(Guid eventId, ITouchData touchData)
        {
            if (!_eventSubjects.TryGetValue(eventId, out BehaviorSubject<TouchEvent> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _eventSubjects.Remove(eventId);
        }
    }
}