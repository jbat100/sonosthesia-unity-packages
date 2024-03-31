using Unity.Collections;
using Unity.Mathematics;
using Sonosthesia.Utils;

namespace Sonosthesia.Mesh
{
    public abstract class ExtrusionPath : ObservableBehaviour
    {
        public abstract float GetLength();

        public abstract bool Populate(NativeArray<RigidTransform> points, float2 range, int segments);
    }
}