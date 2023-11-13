This package provides MIDI data containers and abstract MIDI output and input classes for the sonosthesia project. It is an abstraction layer for different concrete MIDI connectivity layers, which include

- [com.sonosthesia.rtmidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.rtmidi)
- [com.sonosthesia.timelinemidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.timelinemidi)
- [com.sonosthesia.pack](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.pack)

Using this approach the concrete implementation can be switched without affecting other components. The data containers aim to offer a representation of MIDI messages which is more human readable than the raw MIDI bytes.
