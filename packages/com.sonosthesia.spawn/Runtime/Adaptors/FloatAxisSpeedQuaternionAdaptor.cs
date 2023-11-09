using UnityEngine;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class FloatAxisSpeedQuaternionAdaptor : SpeedFloatAdaptor<Quaternion>
    {
        [SerializeField] private Vector3 _axis = Vector3.up;

        protected override Quaternion Map(float value) => Quaternion.AngleAxis(value, _axis);
    }
}