using UnityEngine;

namespace Sonosthesia.Touch
{
    // note : this is deliberately generic, with different providing strategies implemented by TouchActor subclasses
    // we sacrifice specificity for simplicity 

    public enum TouchModulationType
    {
        None,
        Curl,
        Tension,
        Pinch
    }

    public abstract class TouchActorModulator : MonoBehaviour
    {
        public abstract float Select(TouchModulationType modulationType);
    }
}