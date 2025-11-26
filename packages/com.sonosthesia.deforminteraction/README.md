# com.sonosthesia.deforminteraction

Interaction-driven deformations that bridge [deform](https://github.com/jbat100/sonosthesia-unity-packages/tree/main/packages/com.sonosthesia.deform) meshes with the interaction framework. Affordance components react to touch or collision streams and drive noise controllers so meshes pulse, ripple or swell where the interaction occurs.

## Mesh noise affordances

`MeshNoiseAffordance` variants take a `CompoundNoiseMeshController` and a noise configuration and update them every frame while an interaction is active. Spatial falloff settings determine where the deformation originates (actor, source or world space) and how its intensity decays across the surface.

## Envelope-driven parameters

Displacement, radius and speed envelopes are evaluated over the lifetime of the interaction, allowing smooth attacks and releases. Each affordance tracks actor motion to update falloff handles and schedules release timing so the deformation fades out cleanly when the interaction ends.
