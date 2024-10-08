using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchExtractor<T> : ScriptableObject where T : struct
    {
        public abstract ITouchExtractorSession<T> MakeSession();
    }
}