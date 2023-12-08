using System;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class MIDIOutputProxy : MIDIOutput
    {
        [SerializeField] private MIDIOutput _output;

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
            ClearOngoingNotes();
            if (_output)
            {
                _subscription = _output.Pipe(this);
            }
        }
    }
}