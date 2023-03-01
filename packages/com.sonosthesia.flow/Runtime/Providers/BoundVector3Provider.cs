using UnityEngine;

namespace Sonosthesia.Flow
{
    public class BoundVector3Provider : BoundProvider<Vector3>
    {
        protected override Vector3 Randomize(Vector3 lower, Vector3 upper)
        {
            float x = UnityEngine.Random.Range(lower.x, upper.x);
            float y = UnityEngine.Random.Range(lower.y, upper.y);
            float z = UnityEngine.Random.Range(lower.z, upper.z);
            return new Vector3(x, y, z);
        }
    }
}