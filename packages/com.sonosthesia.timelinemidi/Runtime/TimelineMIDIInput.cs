using UnityEngine;
using UnityEngine.Playables;
using Sonosthesia.MIDI;
using Sonosthesia.MIDI.Messages;

namespace Sonosthesia.Timeline.Midi
{
    // Receives MIDI signals (MIDI event notifications) from a timeline and
    // invokes assigned events.
    [ExecuteInEditMode]
    public sealed class TimelineMIDIInput : MIDIInput, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            MidiEvent midiEvent = ((MidiSignal)notification).Event;
            Debug.Log($"Received event {midiEvent} note {midiEvent.IsNote} cc {midiEvent.IsCC} pressure {midiEvent.IsPolyphonicAftertouch}");

            if (midiEvent.IsNoteOn)
            {
                Broadcast(new MIDINote(midiEvent.status & 0x0f, midiEvent.data1, midiEvent.data2));
            }
            else if (midiEvent.IsNoteOff)
            {
                Broadcast(new MIDINote(midiEvent.status & 0x0f, midiEvent.data1, 0));
            }
            else if (midiEvent.IsCC)
            {
                Broadcast(new MIDIControl(midiEvent.status & 0x0f, midiEvent.data1, midiEvent.data2));
            }
            else if (midiEvent.IsPolyphonicAftertouch)
            {
                Broadcast(new MIDIPolyphonicAftertouch(midiEvent.status & 0x0f, midiEvent.data1, midiEvent.data2));
            }
        }
    }
}
