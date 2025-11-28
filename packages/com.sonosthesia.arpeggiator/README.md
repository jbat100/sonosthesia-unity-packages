# com.sonosthesia.arpeggiator

Channel arpeggiators for the sonosthesia project. This package generalizes the concept of a MIDI arpeggiator to any data type: it listens to an incoming [channel](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.channel), duplicates each stream and applies per-step modulation before re-emitting the stream as a new channel.

## Modulation building blocks

Modulators let you sculpt how values evolve across the arpeggio. Float, vector and colour modulators combine animation curves with randomization to create offsets for each step. Followers (for floats, colours and vectors) apply the modulation to a specific property so the generated streams inherit the desired behaviour without bespoke scripts.

## Scheduling and termination

`StaticArpeggiator` and `ScheduledArpeggiator` drive when new steps are emitted. Terminators such as `ArpeggiatorTimerTerminator` or `UnitArpeggiatorTerminator` control when each stream completes, ensuring downstream components see clean lifecycle events.

## Integration points

Arpeggiators use UniRx observables under the hood so they can sit anywhere a channel is expected. Components like `Aftertoucher` make it easy to modulate aftertouch-style data in response to arpeggiated steps, allowing expressive playback without tying code to a specific payload type.
