using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct DeformPathJob : IJobFor
    {
        public NativeArray<RigidTransform> points;
        public NativeArray<float> deformations;
        public float3 direction;

        public void Execute(int index)
        {
            RigidTransform point = points[index];
            float deformation = deformations[index];
            point.pos += math.mul(point.rot, direction) * deformation;
            points[index] = point;
        }
    }
}