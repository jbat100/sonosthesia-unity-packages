# sonosthesia-unity-packages

## Introduction

This repository contains Unity packages for the Sonosthesia project. It aims to provide modular and composable tools to create immersive and interactive audio visual experiences.

## Packages

### Signals

Signals are the main building blocks of the Sonosthesia framework architecture. They are based on [UniRx](https://github.com/neuecc/UniRx) data streams and used to generate, modify, map, receive and transfer data between different components. Signals are templated to allow for use with any value type.

- [com.sonosthesia.signal](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.signal)
- [com.sonosthesia.generator](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.generator)
- [com.sonosthesia.flow](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.flow)
- [com.sonosthesia.target](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.target)

### Channels

Channels are an extension of the MIDI channel concepts, which create and destroy [UniRx](https://github.com/neuecc/UniRx) data streams dynamically. Channels are templated to allow for use with any value type.

- [com.sonosthesia.channel](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.channel)
- [com.sonosthesia.arpeggiator](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.arpeggiator)
- [com.sonosthesia.sequencer](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.sequencer)

### Mapping

Mappings allow signals and channels to be connected to each other, potentially accross heterogeneous data types. They are used to create data flows accross different domains (sound, MIDI, graphics, touch).

- [com.sonosthesia.mapping](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.mapping)
- [com.sonosthesia.link](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.link)

### Input and Interaction

Interactions in XR are crucial to the project, multi modal interaction is encouraged.

- [com.sonosthesia.touch](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.touch)

### Procedural Graphics 

- [com.sonosthesia.noise](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.noise)
- [com.sonosthesia.mesh](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.mesh)
- [com.sonosthesia.deform](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.deform)

### MIDI

- [com.sonosthesia.adaptivemidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.adaptivemidi)
- [com.sonosthesia.rtmidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.rtmidi)
- [com.sonosthesia.timelinemidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.timelinemidi)

### Sound

- [com.sonosthesia.audio](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.audio)

### Instruments

- [com.sonosthesia.instrument](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.instrument)

### Miscellaneous

- [com.sonosthesia.pack](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.pack)
- [com.sonosthesia.processing](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.processing)


## Installation

These packages are hosted on npm and have a dependency on [UniTask](https://github.com/Cysharp/UniTask) and/or [UniRx](https://github.com/neuecc/UniRx). In order to add them to your dependencies, you must add the following scoped registeries.

```json
{
  "scopedRegistries": [
    {
      "name": "Neuecc",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.neuecc.unirx",
        "com.cysharp.unitask"
      ]
    },
    {
      "name": "Sonosthesia",
      "url": "https://registry.npmjs.com",
      "scopes": [
        "com.sonosthesia"
      ]
    }
  ]
}
```

Some packages required additional scoped registeries

- [com.sonosthesia.rtmidi](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.rtmidi)

```json
{
  "scopedRegistries": [
    // ...
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ 
        "jp.keijiro" 
      ]
    }
  ]
}
```

