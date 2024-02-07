using UnityEngine;

namespace Sonosthesia.Instrument
{
    public abstract class MIDINoteFilter : MonoBehaviour
    {
        public abstract bool Allow(int note);
    }
}