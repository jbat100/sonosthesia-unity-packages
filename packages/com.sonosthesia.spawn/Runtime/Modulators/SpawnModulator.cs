using Sonosthesia.Arpeggiator;
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
            RigidTransform rigidTransform = _trans.Modulate(original, p => p.Trans, offset);

            RigidTransform trans = _trans.Modulate(original, p => p.Trans, offset);
            float size = _size.Modulate(original, p => p.Size, offset);
            float lifetime = _lifetime.Modulate(original, p => p.Lifetime, offset);
            Color color = _color.Modulate(original, p => p.Color, offset);
            Vector3 velocity = _velocity.Modulate(original, p => p.Velocity, offset);;
            Vector3 angularVelocity = _angularVelocity.Modulate(original, p => p.AngularVelocity, offset);;
            return new SpawnPayload(trans, size, lifetime, color, velocity, angularVelocity);
        }
    }
}