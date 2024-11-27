using System;
using UnityEngine;
using FMODUnity;
using Sonosthesia.Audio;
using Sonosthesia.Utils;

namespace Sonosthesia.FMOD
{
    [Serializable]
    public class FMODPlayable : IPlayable
    {
        [SerializeField] private StudioEventEmitter _emitter;

        public FMODPlayable(StudioEventEmitter emitter)
        {
            _emitter = emitter;
        }
        
        public void Play()
        {
            if (_emitter)
            {
                _emitter.Play();
            }
        }
    }
    
    public class FMODAudioController : AudioController<FMODPlayable>
    {
#if UNITY_EDITOR
        public void AutofillSlots(bool recursive)
        {
            transform.ComponentScan<StudioEventEmitter>(recursive, 
                check => !Has(check),
                (childName, component) => 
                    Register(new Setting(childName.ToLower(), new FMODPlayable(component))));
        }
        
        public void DeleteAllSlots()
        {
            Clear();
        }
#endif
    }
}