using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public abstract class MIDIPitchTouchExtractor : ScriptableObject
    {
        public abstract bool TryExtract(TouchEvent e, out int pitch);
    }
}