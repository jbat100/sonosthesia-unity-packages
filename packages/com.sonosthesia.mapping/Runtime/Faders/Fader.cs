using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class Fader<T> : MonoBehaviour where T : struct
    {
        public abstract T Fade(float fade);
    }
}