using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public abstract class PathProcessor : MonoBehaviour
    {
        public abstract void Process(NativeArray<RigidTransform> points);
    }
}