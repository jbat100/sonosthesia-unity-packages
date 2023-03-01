using UnityEngine;

namespace Sonosthesia.Flow
{
    public class XYZVectorModulator : Modulator<Vector3>
    {
        [SerializeField] private FloatModulator _x;
        [SerializeField] private FloatModulator _y;
        [SerializeField] private FloatModulator _z;

        public override Vector3 Modulate(Vector3 original, float offset)
        {
            float x = _x ? _x.Modulate(original.x, offset) : original.x;
            float y = _y ? _x.Modulate(original.y, offset) : original.y;
            float z = _z ? _x.Modulate(original.z, offset) : original.z;
            return new Vector3(x, y, z);
        }
    }
}