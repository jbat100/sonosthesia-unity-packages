# sonosthesia-unity-packages

## Introduction

This repository contains Unity packages for the Sonosthesia project. It aims to provide modular and composable tools to create immersive and interactive audio visual experiences.

## Packages

### Signals

Signals are the main building blocks of the Sonosthesia framework architecture. They are based on [UniRx](https://github.com/neuecc/UniRx) data streams and used to generate, modify, map, receive and transfer data between different components.

- [com.sonosthesia.signal](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.signal)
- [com.sonosthesia.target](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.target)

### Channels

Channels are an extension of the MIDI channel concepts, which create and destroy [UniRx](https://github.com/neuecc/UniRx) data streams dynamically. 

- [com.sonosthesia.channel](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.channel)
- [com.sonosthesia.arpeggiator](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.arpeggiator)

### Mapping

- [com.sonosthesia.mapping](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.mapping)
- [com.sonosthesia.link](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.link)

### Input and Interaction

### Procedural Graphics 

### Sound and MIDI


## Installation

These packages have a dependency on [UniTask](https://github.com/Cysharp/UniTask) and [UniRx](https://github.com/neuecc/UniRx). 

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