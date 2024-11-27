using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TouchActor : TouchEndpoint
    {
        [SerializeField] private TouchActorModulator _modulator;
        public TouchActorModulator Modulator => _modulator;
    }
}