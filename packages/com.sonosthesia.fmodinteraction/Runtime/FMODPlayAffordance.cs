using System;
using FMODUnity;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.FMODInteraction
{
    public class FMODPlayAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct
    {
        [SerializeField] private StudioEventEmitter _emitter;
        
        private class Controller : AffordanceController<TEvent, FMODPlayAffordance<TEvent>>, IDisposable
        {
            public Controller(Guid eventId, FMODPlayAffordance<TEvent> affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TEvent e)
            {
                base.Setup(e);

                if (Affordance._emitter)
                {
                    Affordance._emitter.Play();
                }
            }

            public void Dispose()
            {
            }
        }

        protected override IObserver<TEvent> MakeController(Guid id) => new Controller(id, this);
    }
}