using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchGate : MonoBehaviour   
    {
        public abstract bool Check(TouchSource source, TouchActor actor);
    }
}