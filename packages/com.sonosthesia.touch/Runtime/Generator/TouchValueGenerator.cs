using UnityEngine;

namespace Sonosthesia.Touch
{
    /// <summary>
    /// Generates a value for a TriggerChannelSource
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class TouchValueGenerator<TValue> : MonoBehaviour where TValue : struct
    {
        public abstract bool BeginTouch(ITouchData touchData, out TValue value);
        
        public abstract bool UpdateTouch(ITouchData touchData, out TValue value);

        public abstract void EndTouch(ITouchData touchData);
    }
    
    public static class TouchValueGeneratorExtensions
    {
        public static bool ProcessTrigger<TValue>(this TouchValueGenerator<TValue> generator,
            bool initial, ITouchData touchData, out TValue value) where TValue : struct
        {
            return initial ? 
                generator.BeginTouch(touchData, out value) : 
                generator.UpdateTouch(touchData, out value);
        }
    }
}