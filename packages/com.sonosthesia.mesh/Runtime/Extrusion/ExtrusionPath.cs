using Unity.Collections;
using Unity.Mathematics;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [ExecuteAlways]
    public abstract class ExtrusionPath : ObservableBehaviour
    {
        [SerializeField] private bool _closed;
        public bool Closed => _closed;
        
        public abstract float GetLength();

        public abstract bool Populate(NativeArray<RigidTransform> points, float2 range, int segments);
    }

    public static class ExtrusionPathExtensions
    {
        public static void CalculateRotations(this NativeArray<RigidTransform> points, float2 range)
        {
            bool closed = range.x == 0f && Mathf.Abs(range.y - 1f) < 1e-6;

            int lengthMinusOne = points.Length - 1;

            quaternion ComputeRotation(float3 current, float3 next)
            {
                float3 direction = math.normalize(next - current);
             
                // float3 up = Vector3.up;
                // float3 forward = Vector3.forward;
                // float upWeight = 1f - math.abs(math.dot(direction, up));
                // float forwardWeight = 1f - math.abs(math.dot(direction, forward));
                // float3 rotationUp = math.cross(direction, upWeight * up + forwardWeight * forward);

                float3 rotationUp = current;

                return Quaternion.LookRotation(direction, rotationUp);
            }

            for (int index = 0; index < lengthMinusOne; ++index)
            {
                RigidTransform current = points[index];
                RigidTransform next = points[index + 1];
                current.rot = ComputeRotation(current.pos, next.pos);
                points[index] = current;
            }

            if (closed)
            {
                RigidTransform current = points[^1];
                RigidTransform next = points[0];
                current.rot = ComputeRotation(current.pos, next.pos);
                points[^1] = current;
            }
            else
            {
                RigidTransform current = points[^1];
                RigidTransform previous = points[^2];
                current.rot = previous.rot;
                points[^1] = current;
            }
        }
    }
}