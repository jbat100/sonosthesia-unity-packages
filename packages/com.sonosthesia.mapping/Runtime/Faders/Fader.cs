using UnityEngine;

namespace Sonosthesia.Flow
{
    public abstract class Fader<T> : MonoBehaviour where T : struct
    {
        public abstract T Fade(float fade);
    }
}