using System;
using FMODUnity;
using Sonosthesia.Interaction;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.FMODInteraction
{
    public class TouchFMODPlayAffordance : InteractionAffordance<TouchEvent>
    {
        [SerializeField] private StudioEventEmitter _emitter;
        
        private class Controller : AffordanceController<TouchEvent, TouchFMODPlayAffordance>, IDisposable
        {
            public Controller(Guid eventId, TouchFMODPlayAffordance affordance) : base(eventId, affordance)
            {
            }
            
            protected override void Setup(TouchEvent e)
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

        protected override IObserver<TouchEvent> MakeController(Guid id) => new Controller(id, this);
    }
}