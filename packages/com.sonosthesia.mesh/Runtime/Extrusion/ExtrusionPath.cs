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

        public abstract bool Populate(ref NativeArray<RigidTransform> points, float2 range, int segments);
    }
}