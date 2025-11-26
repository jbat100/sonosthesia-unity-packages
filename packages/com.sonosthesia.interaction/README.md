# com.sonosthesia.interaction

Foundational interaction framework for Sonosthesia packages. It defines how sources and actors expose themselves as interaction endpoints, how events gate compatibility, and how affordances consume interaction streams to drive audio, visual, or haptic responses.

## Core interaction model
- `IInteractionEvent` pairs a source and actor endpoint with timing data and utility extensions for working in shared spaces (positions, distances, velocities).【F:packages/com.sonosthesia.interaction/Runtime/IInteractionEvent.cs†L6-L41】【F:packages/com.sonosthesia.interaction/Runtime/Extractors/ExtractionUtils.cs†L64-L145】
- `InteractionEndpoint` implements the shared endpoint surface (domain, interaction layers, dynamics monitor) that actors and sources inherit to participate in interaction checks.【F:packages/com.sonosthesia.interaction/Runtime/InteractionEndpoint.cs†L9-L38】【F:packages/com.sonosthesia.interaction/Runtime/IInteractionEndpoint.cs†L8-L15】
- Gates and layer matching let affordances filter streams before they start, combining per-endpoint gates with `InteractionLayerMatch` rules for coarse compatibility control.【F:packages/com.sonosthesia.interaction/Runtime/Gates/InteractionGate.cs†L5-L15】【F:packages/com.sonosthesia.interaction/Runtime/InteractionLayerMatch.cs†L5-L34】

## Affordance pipeline
- `InteractionAffordance<TEvent>` subscribes to channel streams, evaluates gates, and hands off to controllers or relays while keeping logging and relay wiring unified across packages.【F:packages/com.sonosthesia.interaction/Runtime/Affordances/InteractionAffordance.cs†L12-L110】
- Specialized affordances (drag, peak, trigger, torque, scheduler, channel-count, activation) build on the base to spawn visuals, drive envelopes, or coordinate schedulers while reusing the same stream lifecycle.

## Extractors, triggers, and envelopes
- Extraction utilities provide reusable float and vector extractors (velocity, distance, twist, relative position, axis) plus configurable static/dynamic extractor settings that downstream packages compose for their own event types.【F:packages/com.sonosthesia.interaction/Runtime/Extractors/ExtractionUtils.cs†L9-L346】
- Interactive triggers wrap envelope playback and attack/release extraction so affordances can start, update, and end gestures consistently, whether they are pulses or holds.【F:packages/com.sonosthesia.interaction/Runtime/Triggers/IInteractiveTriggerSettings.cs†L9-L49】【F:packages/com.sonosthesia.interaction/Runtime/Triggers/InteractiveTriggerSession.cs†L10-L72】
- Interactive envelopes mirror the trigger flow for continuous values, supporting bypass or gated playback, optional One Euro filtering, and warpable release envelopes for smooth teardown.【F:packages/com.sonosthesia.interaction/Runtime/Envelopes/InteractiveEnvelopeSession.cs†L9-L186】【F:packages/com.sonosthesia.interaction/Runtime/Envelopes/IInteractiveEnvelopeSettings.cs†L8-L22】

## Dynamic tracking
- Dynamic tracking sessions generate positional targets for moving emitters or falloff handles, from frozen anchors to drag-damped velocity extrapolation using monitored transform dynamics.【F:packages/com.sonosthesia.interaction/Runtime/Tracking/DynamicTrackingSession.cs†L8-L71】【F:packages/com.sonosthesia.interaction/Runtime/Tracking/DynamicTrackingSettings.cs†L7-L18】

## Usage in dependent packages
- **com.sonosthesia.touch** — Uses `InteractionEndpoint`-derived touch actors/sources and `TouchEvent` interaction streams to gate collider contacts and drive touch affordances built on interaction extractors.【F:packages/com.sonosthesia.touch/Runtime/Graph/TouchEvent.cs†L33-L55】【F:packages/com.sonosthesia.touch/Runtime/Source/ATouchSource.cs†L11-L220】
- **com.sonosthesia.collide** — Uses `CollideEvent` interaction data and interaction-based trigger configurations to convert collision streams into affordance-friendly envelopes and torque/peak drivers.【F:packages/com.sonosthesia.collide/Runtime/Sources/CollideEvent.cs†L6-L24】【F:packages/com.sonosthesia.collide/Runtime/Affordances/CollideTriggerConfiguration.cs†L6-L12】
- **com.sonosthesia.pointer** — Uses `PointerSource` endpoints that publish `PointerEvent` interaction streams into channels for pointer trigger, scheduler, and drag affordances built on the interaction base class.【F:packages/com.sonosthesia.pointer/Runtime/Source/PointerSource.cs†L12-L93】【F:packages/com.sonosthesia.pointer/Runtime/Source/PointerEventChannel.cs†L7-L23】
- **com.sonosthesia.instrument** — Uses interaction-affordance controllers and extractor-based MIDI configurations to map interaction events into MIDI notes and control streams.【F:packages/com.sonosthesia.instrument/Runtime/Interaction/MIDINoteAffordance.cs†L10-L80】【F:packages/com.sonosthesia.instrument/Runtime/Extractors/MIDIExtractorSettings.cs†L18-L30】
- **com.sonosthesia.fmodinteraction** — Uses interactive envelope settings and dynamic tracking from interaction configurations to position FMOD emitters and drive volume/excitation/body curves per event.【F:packages/com.sonosthesia.fmodinteraction/Runtime/FMODEmitterConfiguration.cs†L8-L34】
- **com.sonosthesia.deforminteraction** — Uses interaction envelopes and tracking inside mesh noise configurations so touch or collision events modulate falloff, displacement, and speed across deformable surfaces.【F:packages/com.sonosthesia.deforminteraction/Runtime/Mesh/MeshNoiseConfiguration.cs†L8-L45】
- **com.sonosthesia.vfx** — Uses `VFXEventAffordance<TEvent>` to turn interaction streams into VFX Graph event calls with attributes extracted from each interaction payload.【F:packages/com.sonosthesia.vfx/Runtime/Affordances/VFXEventAffordance.cs†L10-L33】
- **com.sonosthesia.sculpt** — Declares the interaction dependency so sculpting tools can share endpoint/gate infrastructure alongside XR interaction components.【F:packages/com.sonosthesia.sculpt/package.json†L1-L17】
