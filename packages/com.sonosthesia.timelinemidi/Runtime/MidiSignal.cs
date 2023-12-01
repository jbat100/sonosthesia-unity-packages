using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Timeline.MIDI
{
    // Payload for MIDI event notifications
    public sealed class MIDISignal : INotification
    {
        // Notification ID (not in use)
        PropertyName INotification.id { get { return default(PropertyName);} }

        // MIDI event
        public MIDIEvent Event { get; set; }
    }
}
