using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public abstract class ParametricPath : ExtrusionPath
    {
        protected const float TAU = 2 * Mathf.PI;

        protected void RecalculateRotations(ref NativeArray<RigidTransform> points, float2 range)
        {
            bool closed = range.x == 0f && Math.Abs(range.y - 1f) < 1e-6;

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