using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace Sonosthesia.Mesh
{
    public class CircleParametricPath : ParametricPath
    {
        [SerializeField] private float _radius = 1f;

        public override float GetLength() => _radius * TAU;

        public override bool Populate(ref NativeArray<RigidTransform> points, float2 range, int segments)
        {
            for (int index = 0; index < segments; ++index)
            {
                float s = index / (segments - 1f);
                float t = math.lerp(range.x, range.y, s) * TAU;

                float3 position = new float3(_radius * math.cos(t), 0f, _radius * math.sin(t));
                quaternion rotation = quaternion.LookRotation(math.cross(position, Vector3.up), Vector3.up);

                points[index] = new RigidTransform(rotation, position);
            }
            
            return true;
        }
    }
}