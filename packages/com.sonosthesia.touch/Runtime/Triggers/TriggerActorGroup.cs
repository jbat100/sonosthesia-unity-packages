using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TriggerActorGroup<TValue> : Register<TriggerActor<TValue>> where TValue : struct
    {
        [SerializeField] private int _maxConcurrent = 1;

        public bool IsAvailable(Collider actor)
        {
            int total = 0;
            foreach (TriggerActor<TValue> triggerActor in Raw)
            {
                total += triggerActor.ValueStates.Count;
                if (total >= _maxConcurrent)
                {
                    return false;
                }
            }
            return true;
        }
    }
}