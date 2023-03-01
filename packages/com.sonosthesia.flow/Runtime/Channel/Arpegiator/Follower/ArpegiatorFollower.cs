using UnityEngine;

namespace Sonosthesia.Flow
{
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