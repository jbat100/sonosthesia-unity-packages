using Sonosthesia.Arpeggiator;
using Unity.Mathematics;

namespace Sonosthesia.Spawn
{
    public class RigidTransformFollower : ArpegiatorFollower<RigidTransform>
    {
        public override RigidTransform Follow(RigidTransform original, RigidTransform current, RigidTransform arpegiated)
        {
            return math.mul(math.mul(current, math.inverse(original)), arpegiated);
        }
    }
}