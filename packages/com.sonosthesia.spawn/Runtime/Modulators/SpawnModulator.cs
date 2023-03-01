using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnModulator : Modulator<SpawnPayload>
    {
        [SerializeField] private Modulator<RigidTransform> _trans;
        [SerializeField] private Modulator<float> _size;
        [SerializeField] private Modulator<float> _lifetime;
        [SerializeField] private Modulator<Color> _color;
        [SerializeField] private Modulator<Vector3> _velocity;
        [SerializeField] private Modulator<Vector3> _angularVelocity;
        
        public override SpawnPayload Modulate(SpawnPayload original, float offset)
        {
            RigidTransform trans = _trans ? _trans.Modulate(original.Trans, offset) : original.Trans;
            float size = _size ? _size.Modulate(original.Size, offset) : original.Size;
            float lifetime = _lifetime ? _lifetime.Modulate(original.Lifetime, offset) : original.Lifetime;
            Color color = _color ? _color.Modulate(original.Color, offset) : original.Color;
            Vector3 velocity = _velocity ? _velocity.Modulate(original.Velocity, offset) : original.Velocity;
            Vector3 angularVelocity = _angularVelocity ? _angularVelocity.Modulate(original.AngularVelocity, offset) : original.AngularVelocity;
            return new SpawnPayload(trans, size, lifetime, color, velocity, angularVelocity);
        }
    }
}