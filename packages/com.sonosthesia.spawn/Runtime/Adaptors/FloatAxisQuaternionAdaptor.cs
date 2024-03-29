using UnityEngine;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class FloatAxisQuaternionAdaptor : FloatMapAdaptor<Quaternion>
    {
        [SerializeField] private Vector3 _axis = Vector3.up;

        protected override Quaternion Map(float value) => Quaternion.AngleAxis(value, _axis);
    }
}