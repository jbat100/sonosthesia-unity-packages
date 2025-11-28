# com.sonosthesia.packaudio

Pack (MessagePack) audio helpers for the sonosthesia project. The package mirrors the audio analysis data used by [pack](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.pack) so that remote tri-band and quint-band streams can be received or generated inside Unity.

## Band splitters

`AudioTriBandSplitter` and `AudioQuintBandSplitter` take spectral analysis and expose float signals for each band. They follow the same addressing scheme as the pack protocol (`/audio/tribands` and `/audio/quintbands`) so the data can be forwarded over WebSocket or stored alongside other packed messages.

## Receivers and signals

`PackAudioTriBandReceiver` and `PackAudioQuintBandReceiver` listen for incoming pack messages and reconstitute them as Unity signals. `AudioBandFloatSignal` and `AudioQuintBandFloatSignal` make the band values available to other signal-aware components without having to parse message payloads manually.
