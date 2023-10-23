using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool ProcessTriggerEnter(Collider other, out TValue value);
        
        public abstract bool ProcessTriggerStay(Collider other, out TValue value);

        public abstract void ProcessTriggerExit(Collider other);
    }
}