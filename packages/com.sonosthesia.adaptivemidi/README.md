# com.sonosthesia.adaptivemidi

This package provides MIDI data containers and abstract MIDI output and input classes for the sonosthesia project. It is an abstraction layer for different concrete MIDI connectivity layers, which include

- [com.sonosthesia.rtmidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.rtmidi)
- [com.sonosthesia.timelinemidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.timelinemidi)
- [com.sonosthesia.pack](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.pack)

Using this approach the concrete implementation can be switched without affecting other components. The data containers aim to offer a representation of MIDI messages which is more human readable than the raw MIDI bytes.

## Supported MIDI Messages

- Note On
- Note Off
- Control Change
- Polyphonic Aftertouch
- Channel Aftertouch
- Channel Pitch Bend
- Song Pointer Position
- Clock
- Start
- Stop
- Continue

## MIDI Messages Representation

MIDI messages are represented using human readable value types rather than raw bytes for better usability. `MIDIEncoder` and `MIDIDecoder` are used to convert to and from raw bytes when needed.

## MIDIOutput

See example usage here.

### MIDIInput API

See example usage here.