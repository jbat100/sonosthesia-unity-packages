using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class AgnosticTouchSource : TouchSource
    {
        private readonly Dictionary<Guid, BehaviorSubject<TouchEvent>> _eventSubjects = new();

        protected override bool ConfigureStream(Guid id, ITouchData touchData)
        {
            TouchEvent sourceEvent = new TouchEvent(touchData, Time.time);
            BehaviorSubject<TouchEvent> eventSubject = new BehaviorSubject<TouchEvent>(sourceEvent);
            _eventSubjects[id] = eventSubject;
            IObservable<TouchEvent> sourceObservable = eventSubject.AsObservable();
            if (Node)
            {
                Node.StreamNode.Push(id, sourceObservable);
            }
            // push the stream to the actor
            if (sourceEvent.TouchData.Actor && sourceEvent.TouchData.Actor.Node)
            {
                sourceEvent.TouchData.Actor.Node.StreamNode.Push(id, sourceObservable);
            }
            return true;
        }

        protected override void UpdateStream(Guid id, ITouchData touchData)
        {
            if (!_eventSubjects.TryGetValue(id, out BehaviorSubject<TouchEvent> subject))
            {
                return;
            }
            subject.OnNext(subject.Value);
        }

        protected override void CleanupStream(Guid id, ITouchData touchData)
        {
            if (!_eventSubjects.TryGetValue(id, out BehaviorSubject<TouchEvent> subject))
            {
                return;
            }
            
            subject.OnCompleted();
            subject.Dispose();

            _eventSubjects.Remove(id);
        }
    }
}