using FMOD;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public abstract class FMODSynthSource : MonoBehaviour
    {
        public abstract bool CreateSource(DSP target);

        public abstract void Cleanup();

        protected abstract bool Apply();

        protected virtual void OnValidate() => Apply();
    }
}