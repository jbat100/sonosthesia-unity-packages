using UnityEngine;
using Sonosthesia.Flow;
using Sonosthesia.Utils;

namespace Sonosthesia.Spawn
{
    public class FloatPathSpeedTransAdaptor : SpeedFloatAdaptor<Trans>
    {
        [SerializeField] private Path _path;
        
        protected override Trans Map(float value)
        {
            return new Trans(_path.GetPosition(value, true), _path.GetRotation(value, true), Vector3.one);
        }
    }
}