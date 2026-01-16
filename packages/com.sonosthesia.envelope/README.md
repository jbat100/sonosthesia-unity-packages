
# com.sonosthesia.envelope

Envelope primitives and utilities for the Sonosthesia framework. Provides reusable envelope implementations (ADSR and variants, animation-curve, constant), runtime factories, and editor tooling so audio/visual affordances can stay in sync with timing and easing data.

## Envelope primitives
- `IEnvelope` exposes duration, initial/final values, and an `Evaluate(time)` API for any time-based curve you want to drive. [IEnvelope.cs](packages/com.sonosthesia.envelope/Runtime/IEnvelope.cs)
- `EnvelopePhase` couples an `EaseType` with a duration; phased envelopes compose phases into A, AHR, ADSR, ADS, and SR shapes for common attack/decay workflows. [PhasedEnvelope.cs](packages/com.sonosthesia.envelope/Runtime/Envelopes/PhasedEnvelope.cs)
- Additional implementations cover Unity animation curves, constant values, and `WarpedEnvelope` to scale time and amplitude without rewriting the source envelope. [AnimationCurveEnvelope.cs](packages/com.sonosthesia.envelope/Runtime/Envelopes/AnimationCurveEnvelope.cs) [ConstantEnvelope.cs](packages/com.sonosthesia.envelope/Runtime/Envelopes/ConstantEnvelope.cs) [WarpedEnvelope.cs](packages/com.sonosthesia.envelope/Runtime/Envelopes/WarpedEnvelope.cs)

## Configuration and factories
- `EnvelopeSettings` selects an envelope type (phased, curve, constant, or custom factory), exposes helpers to build ADSR/AHR/ADS/SR presets, and includes an extension that constructs the runtime envelope on demand. [EnvelopeSettings.cs](packages/com.sonosthesia.envelope/Runtime/EnvelopeSettings.cs)
- `EnvelopeFactory` is a `ScriptableObject` wrapper around `EnvelopeSettings` so scenes and prefabs can instantiate envelopes at runtime; legacy `EnvelopeBuilder` components remain for backward compatibility. [EnvelopeFactory.cs](packages/com.sonosthesia.envelope/Runtime/EnvelopeFactory.cs) [EnvelopeBuilder.cs](packages/com.sonosthesia.envelope/Runtime/Builders/EnvelopeBuilder.cs)

## Editor tooling
- Custom property drawers render envelope settings with context-aware visibility and compact phase editors, keeping inspector layouts tidy when switching between phased/curve/constant/custom modes. [EnvelopeSettingsDrawer.cs](packages/com.sonosthesia.envelope/Editor/EnvelopeSettingsDrawer.cs) [EnvelopePhaseDrawer.cs](packages/com.sonosthesia.envelope/Editor/EnvelopePhaseDrawer.cs)

## Usage in dependent packages
- `com.sonosthesia.interaction` uses `EnvelopeSettings` for interactive envelopes and triggers, warping attack/release timing per interaction and optionally filtering via One Euro smoothing. [IInteractiveEnvelopeSettings.cs](packages/com.sonosthesia.interaction/Runtime/Envelopes/IInteractiveEnvelopeSettings.cs) [IInteractiveTriggerSettings.cs](packages/com.sonosthesia.interaction/Runtime/Triggers/IInteractiveTriggerSettings.cs)
- `com.sonosthesia.trigger` builds trigger playback envelopes from extracted values (attack/sustain/release) before driving signal-based trigger controllers. [SignalTriggerConfiguration.cs](packages/com.sonosthesia.trigger/Runtime/Signal/SignalTriggerConfiguration.cs)
- `com.sonosthesia.fmodinteraction` feeds interactive envelopes into FMOD emitter configurations so volume/excitation/body parameters follow event-driven curves rather than static values. [FMODEmitterConfiguration.cs](packages/com.sonosthesia.fmodinteraction/Runtime/FMODEmitterConfiguration.cs)
- `com.sonosthesia.deforminteraction` and `com.sonosthesia.collide` extend the interactive envelope settings to modulate mesh noise, falloff, and collision responses with ADSR-style curves per event type. [MeshNoiseConfiguration.cs](packages/com.sonosthesia.deforminteraction/Runtime/Mesh/MeshNoiseConfiguration.cs) [CollidePeakConfiguration.cs](packages/com.sonosthesia.collide/Runtime/Affordances/CollidePeakConfiguration.cs)
