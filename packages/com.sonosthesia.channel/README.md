# com.sonosthesia.channel

Core channels implementation for the sonosthesia project. Channels are an extension of the concept of MIDI channels which create (note on), modify (aftertouch) and destroy (note off) notes. When extending this concept to arbitrary data structures, event stream are created, modified and destroyed and any field held by the data can evolve during the stream lifetime.

To a first approximation a channel can be described (and is implemented) in [Rx](https://github.com/neuecc/UniRx) by

```
public interface IChannel<T> 
{
    IObservable<IObservable<T>> StreamObservable { get; }
}
```

`StreamObservable` generates `IObservable<T>` instances which are expected to update `T` and complete once the stream ends. 
