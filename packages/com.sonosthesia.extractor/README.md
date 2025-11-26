# com.sonosthesia.extractor

Extraction helpers for turning interaction events into normalized values that can drive other Sonosthesia systems.

## Overview

This package provides static and dynamic extractors that convert incoming events into typed data (for example `float`, `Vector3`, or other structs you add). Extractors can be composed with processors from `com.sonosthesia.processing` and configured as `ScriptableObject` assets to re-use extraction strategies across scenes.

## Static extractors

Static extractors (`IStaticExtractor`) map a single event to a value, which is useful when only the first event is available (for example MIDI note properties). The `StaticExtractorSettings` base class is generic so you can target any data type and lets you combine:

- A raw extraction strategy, such as `PeakFloatStaticExtractorSettings` or `PeakVectorStaticExtractorSettings` for selecting peak magnitude, strength, duration, a constant value, or a custom extractor tailored to the output type.
- Post-processing via the configured `IProcessor` instance.
- Optional composition with other extractors through the `InterfaceReference` field.

## Dynamic extractors

Dynamic extractors (`IDynamicExtractor`) create sessions that react to event streams over time. The `DynamicExtractorSettings` pipeline is also generic so it can be applied to any structured value. Built-in follow strategies include:

- **Float**: `Initial`, `Track`, `Relative`, and `Normalized` via `FloatDynamicExtractorSettings` (relative and normalized variants derive values from the first sample).
- **Vector**: `Initial`, `Track`, and `Relative` via `VectorDynamicExtractorSettings`.
- **Custom**: implement your own typed follow strategies by extending `DynamicExtractorSettings<TValue>` and pairing it with a session processor suited to your data.

Session processors such as `FuncExtractionSessionProcessor`, `InitialSession`, and `RelativeSession` make it easy to wrap custom logic around existing sessions while keeping implementations stateless when needed.

## Customization

Both static and dynamic extractors can be authored as plain C# classes implementing `IStaticExtractor` or `IDynamicExtractor`, then wrapped in `SettingsStaticExtractor` or `SettingsDynamicExtractor` so they can be serialized as assets. Editor drawers (`FloatStaticExtractorSettingsDrawer`, `FloatDynamicExtractorSettingsDrawer`) provide an inspector-friendly way to configure follow strategy, processors, and custom extractor references.

## Usage in dependent packages

Other Sonosthesia packages build on these extractors to translate domain events into reusable control signals:

- **Pointer**: Provides `FloatPointerStaticExtractorSettings` and `FloatPointerDynamicExtractorSettings` that pull pressure, scroll, raycast, or screen-space data from `PointerEvent` streams, and feeds those values into affordances such as `PointerTriggerConfiguration`.【F:packages/com.sonosthesia.pointer/Runtime/Extractor/FloatPointerStaticExtractorSettings.cs†L10-L41】【F:packages/com.sonosthesia.pointer/Runtime/Extractor/FloatPointerDynamicExtractorSettings.cs†L9-L104】【F:packages/com.sonosthesia.pointer/Runtime/Affordance/PointerTriggerConfiguration.cs†L6-L9】
- **Touch**: Provides static and dynamic extractor settings for `TouchEvent` that modulate values based on actor data, and routes the outputs into trigger/affordance configurations like `TouchTriggerConfiguration`.【F:packages/com.sonosthesia.touch/Runtime/Extractor/FloatTouchDynamicExtractorSettings.cs†L9-L43】【F:packages/com.sonosthesia.touch/Runtime/Extractor/FloatTouchStaticExtractorSettings.cs†L7-L12】【F:packages/com.sonosthesia.touch/Runtime/Affordance/TouchTriggerConfiguration.cs†L6-L9】
- **Interaction**: Consumes `IStaticExtractor` and `IDynamicExtractor` outputs to drive envelope playback, pairing dynamic value extraction with static attack/release extraction inside `InteractiveTriggerSettings`.【F:packages/com.sonosthesia.interaction/Runtime/Triggers/IInteractiveTriggerSettings.cs†L15-L48】
- **Trigger**: Consumes multiple static extractor outputs inside `SignalTriggerConfiguration` to calculate value and timing before firing envelopes.【F:packages/com.sonosthesia.trigger/Runtime/Signal/SignalTriggerConfiguration.cs†L7-L38】
- **VFX**: Consumes static extractor outputs in `VFXEventAffordance` to set per-attribute values before dispatching visual effect events.【F:packages/com.sonosthesia.vfx/Runtime/Affordances/VFXEventAffordance.cs†L9-L38】
- **Instrument**: Consumes touch extractor outputs in `TouchMIDINoteConfiguration` to derive channel, pitch, and expressive MIDI data from touch events.【F:packages/com.sonosthesia.instrument/Runtime/Touch/TouchMIDINoteConfiguration.cs†L6-L13】
