using Sonosthesia.Arpeggiator;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnFollower : ArpegiatorFollower<SpawnPayload>
    {
        [SerializeField] private ArpegiatorFollower<RigidTransform> _transform;
        [SerializeField] private ArpegiatorFollower<float> _size;
        [SerializeField] private ArpegiatorFollower<Color> _color;

        public override SpawnPayload Follow(SpawnPayload original, SpawnPayload current, SpawnPayload arpegiated)
        {
            RigidTransform rigidTransform = _transform.Follow(p => p.Trans, original, current, arpegiated);
            float size = _size.Follow(p => p.Size, original, current, arpegiated);
            Color color = _color.Follow(p => p.Color, original, current, arpegiated);
            return new SpawnPayload(rigidTransform, size, arpegiated.Lifetime, color, arpegiated.Velocity, arpegiated.AngularVelocity);
        }
    }
}