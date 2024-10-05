using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchGate : MonoBehaviour   
    {
        public abstract bool AllowTrigger(TouchEndpoint source, TouchEndpoint actor);
    }
}