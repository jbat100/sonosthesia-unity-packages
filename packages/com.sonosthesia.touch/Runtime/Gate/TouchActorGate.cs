using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchActorGate : MonoBehaviour
    {
        [SerializeField] private bool _bypass;

        public bool Check(ATouchSource source, TouchActor actor)
        {
            return _bypass || PerformCheck(source, actor);
        }
        
        protected abstract bool PerformCheck(ATouchSource source, TouchActor actor);
    }
}