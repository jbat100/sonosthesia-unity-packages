# com.sonosthesia.fmod

FMOD integration helpers for the sonosthesia project. The package wraps FMOD Studio components so they can participate in signal, channel and interaction graphs without bespoke glue code.

## Playback and routing

Utility components such as `FMODSound`, `FMODEventReferenceSound` and `FMODEmitter` provide simple ways to trigger events while exposing parameters that can be driven by signals. Mixer utilities (`VCAVolume`, `BusVolume`, `Volume`) make it easy to alter buses or VCAs at runtime and keep levels in sync with other audio sources.

## Processing

`FMODProcessor` and `FMODDSPProcessor` offer a lifecycle-aware hook into FMOD channel groups. They let you attach processors to live channel groups, configure DSP behaviour, and cleanly tear down the processing pipeline when sources stop.

## Debugging and testing

Test helpers like `FMODCoreTest`, `FMODInstanceDebug` and `FMODTrackVolume` make it straightforward to validate event routing and runtime parameter changes inside Unity scenes.
