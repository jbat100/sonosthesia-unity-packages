using UnityEngine;
using System.Collections.Generic;

namespace Sonosthesia.Timeline.MIDI
{
    // Object pool class for MIDI signals
    sealed class MIDISignalPool
    {
        Stack<MIDISignal> _usedSignals = new Stack<MIDISignal>();
        Stack<MIDISignal> _freeSignals = new Stack<MIDISignal>();

        public MIDISignal Allocate(in MIDIEvent data)
        {
            var signal = _freeSignals.Count > 0 ?  _freeSignals.Pop() : new MIDISignal();
            signal.Event = data;
            _usedSignals.Push(signal);
            return signal;
        }

        public void ResetFrame()
        {
            while (_usedSignals.Count > 0) _freeSignals.Push(_usedSignals.Pop());
        }
    }
}
