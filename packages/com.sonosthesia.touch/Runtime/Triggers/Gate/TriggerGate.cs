using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerGate : MonoBehaviour   
    {
        public abstract bool AllowTrigger(BaseTriggerSource source, BaseTriggerActor actor);
    }
}