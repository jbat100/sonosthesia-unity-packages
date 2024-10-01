using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class CollisionValueGenerator<T> : MonoBehaviour where T : struct
    {
        public abstract T GetValue(Collision collision, TransformDynamics dynamics);
    }

    public static class CollisionValueProviderExtensions
    {
        public static int GetIntValue(this CollisionValueGenerator<float> generator, Collision collision, TransformDynamics dynamics)
        {
            return Mathf.RoundToInt(generator.GetValue(collision, dynamics));
        }
        
        public static int GetIntValue(this CollisionValueGenerator<float> generator, Collision collision, TransformDynamics dynamics, float min, float max)
        {
            return Mathf.RoundToInt(Mathf.Clamp(generator.GetValue(collision, dynamics), min, max));
        }
    }
}