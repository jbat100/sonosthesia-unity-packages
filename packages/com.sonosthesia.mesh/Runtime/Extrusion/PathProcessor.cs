using Sonosthesia.Utils;
using Unity.Collections;
using Unity.Mathematics;

namespace Sonosthesia.Mesh
{
    public abstract class PathProcessor : ObservableBehaviour
    {
        public abstract void Process(NativeArray<RigidTransform> points);
    }
}