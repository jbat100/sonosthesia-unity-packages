using Unity.Mathematics;

namespace Sonosthesia.Utils
{
    public static class RigidTransformExtension
    {
        public static RigidTransform RelativeTo(this RigidTransform velocity, RigidTransform other)
        {
            float3 pos = velocity.pos - other.pos;
            quaternion rot = math.mul(velocity.rot, math.inverse(other.rot));
            return new RigidTransform(rot, pos);
        }
    }
}