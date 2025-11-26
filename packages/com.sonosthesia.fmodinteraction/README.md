# com.sonosthesia.fmodinteraction

Interaction affordances that drive FMOD emitters. The package connects the [touch](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.touch) and [collide](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.collide) interaction streams to FMOD events so spatial input can trigger audio in a controlled way.

## Configurable emitters

`FMODEmitterConfiguration` assets capture the event reference, parameters and spatial behaviour required for playback. Specialised configurations for touch and collision let you author different responses for pointer-based input versus physics events.

## Play and parameter affordances

`FMODPlayAffordance` components launch or stop events when an interaction begins or ends. `FMODEmitterAffordance` variants expose Unity events for custom bindings, while `CollideFMODEmitterAffordance` and `TouchFMODEmitterAffordance` provide ready-made hooks for common interaction types.
