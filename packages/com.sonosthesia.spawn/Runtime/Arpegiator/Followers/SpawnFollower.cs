using Sonosthesia.Flow;
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
            RigidTransform rigidTransform = _transform ? _transform.Follow(original.Trans, current.Trans, arpegiated.Trans) : arpegiated.Trans;
            float size = _size ? _size.Follow(original.Size, current.Size, arpegiated.Size) : arpegiated.Size;
            Color color = _color ? _color.Follow(original.Color, current.Color, arpegiated.Color) : arpegiated.Color;
            return new SpawnPayload(rigidTransform, size, arpegiated.Lifetime, color, arpegiated.Velocity, arpegiated.AngularVelocity);
        }
    }
}