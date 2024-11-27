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
        
        protected override void HandleStream(Guid id, IObservable<TouchEvent> stream)
        {
            if (_emitter)
            {
                _emitter.Play();
            }
        }
    }
}