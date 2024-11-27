using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchActorGate : MonoBehaviour
    {
        [SerializeField] private bool _bypass;

        public bool Check(TouchSource source, TouchActor actor)
        {
            return _bypass || PerformCheck(source, actor);
        }
        
        protected abstract bool PerformCheck(TouchSource source, TouchActor actor);
    }
}