using UnityEngine;
using UnityEngine.Playables;
using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Timeline.MIDI
{
    // Receives MIDI signals (MIDI event notifications) from a timeline and invokes assigned events.
    [ExecuteInEditMode]
    public sealed class TimelineMIDIInput : MIDIInput, INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            MIDIEvent midiEvent = ((MIDISignal)notification).Event;
            Debug.Log($"Received event {midiEvent} note {midiEvent.IsNote} cc {midiEvent.IsCC} pressure {midiEvent.IsPolyphonicAftertouch}");

            if (midiEvent.IsNoteOn)
            {
                Broadcast(new MIDINoteOn(midiEvent.status & 0x0f, midiEvent.data1, midiEvent.data2));
            }
            else if (midiEvent.IsNoteOff)
            {
                Broadcast(new MIDINoteOff(midiEvent.status & 0x0f, midiEvent.data1, 0));
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
