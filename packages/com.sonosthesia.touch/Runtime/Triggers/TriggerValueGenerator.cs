using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TriggerValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool ProcessTriggerEnter(Collider other, out TValue value);
        
        public abstract bool ProcessTriggerStay(Collider other, out TValue value);

        public abstract void ProcessTriggerExit(Collider other);
    }
    
    public static class TriggerValueGeneratorExtensions
    {
        public static bool ProcessTrigger<TValue>(this TriggerValueGenerator<TValue> generator,
            bool initial, Collider other, out TValue value) where TValue : struct
        {
            return initial ? 
                generator.ProcessTriggerEnter(other, out value) : 
                generator.ProcessTriggerStay(other, out value);
        }
    }
}