using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIInputProxy : MIDIInput
    {
        [SerializeField] private MIDIInput _input;

        private IDisposable _subscription;

        protected virtual void OnValidate()
        {
            ReloadPipe();
        }

        protected virtual void OnEnable()
        {
            ReloadPipe();
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        private void ReloadPipe()
        {
            _subscription?.Dispose();
            if (_input)
            {
                _subscription = Pipe(_input);    
            }
        }
    }
}