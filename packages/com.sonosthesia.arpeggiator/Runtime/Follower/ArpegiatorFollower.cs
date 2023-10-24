using System;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public static class ArpegiatorFollowerExtensions
    {
        public static U Follow<T, U>(this ArpegiatorFollower<U> follower, Func<T, U> selector, 
            T original, T current, T arpegiated) where T : struct where U : struct
        {
            return follower ? 
                follower.Follow(selector(original), selector(current), selector(arpegiated)) : 
                selector(arpegiated);
        }
    }
    
    public abstract class ArpegiatorFollower<T> : MonoBehaviour where T : struct
    {
        /// <summary>
        /// Determine following value
        /// </summary>
        /// <param name="original">Original stream value at arpegiation start</param>
        /// <param name="current">Current stream value</param>
        /// <param name="arpegiated">Original arpegiated value</param>
        /// <returns></returns>
        public abstract T Follow(T original, T current, T arpegiated);
    }
}