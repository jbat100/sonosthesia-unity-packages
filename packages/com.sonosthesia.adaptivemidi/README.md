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
- Start/Stop/Continue

## MIDI Messages Representation

MIDI messages are represented using human readable [value types](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.adaptivemidi/Runtime/Messages) rather than raw bytes for better usability. `MIDIEncoder` and `MIDIDecoder` are used to convert to and from raw bytes when needed.

Input API makes use of [UniRx](https://github.com/neuecc/UniRx) to present incoming messages as data streams.

## Examples

See example API usage for MIDI [input](https://github.com/jbat100/sonosthesia-unity-packages/blob/main/packages/com.sonosthesia.adaptivemidi/Runtime/Examples/MIDIInputExample.cs) and [output](https://github.com/jbat100/sonosthesia-unity-packages/blob/main/packages/com.sonosthesia.adaptivemidi/Runtime/Examples/MIDIOutputExample.cs) and [demo application](https://github.com/jbat100/sonosthesia-unity-demo-midi/tree/main)
