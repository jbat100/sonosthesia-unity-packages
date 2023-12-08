# com.sonosthesia.signal

Signals for the Sonosthesia project. Signals are the main building blocks of the Sonosthesia framework architecture. They are based on [UniRx](https://github.com/neuecc/UniRx) data streams and used to generate, modify, map, receive and transfer data between different components. Signals are templated to allow for use with any value type.

## Signals API

The API is purposefully minimalistic. It exposes a `SignalObservable` and a `Broadcast` method.

## Relays

`SignalRelay` are `ScriptableObjects` which allow signals to be fed accross Scene boundaries, from and to Prefab instances. 

## Targets

Targets are signal receivers which implement reactive behaviour.