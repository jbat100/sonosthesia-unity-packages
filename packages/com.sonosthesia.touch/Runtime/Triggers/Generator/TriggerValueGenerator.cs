using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Generates a value for a TriggerChannelSource
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class TriggerValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool BeginTrigger(ITriggerData triggerData, out TValue value);
        
        public abstract bool UpdateTrigger(ITriggerData triggerData, out TValue value);

        public abstract void EndTrigger(ITriggerData triggerData);
    }
    
    public static class TriggerValueGeneratorExtensions
    {
        public static bool ProcessTrigger<TValue>(this TriggerValueGenerator<TValue> generator,
            bool initial, ITriggerData triggerData, out TValue value) where TValue : struct
        {
            return initial ? 
                generator.BeginTrigger(triggerData, out value) : 
                generator.UpdateTrigger(triggerData, out value);
        }
    }
}