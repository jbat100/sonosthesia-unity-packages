using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch.Providers
{
    public abstract class CollisionValueProvider<T> : MonoBehaviour where T : struct
    {
        public abstract T GetValue(Collision collision, TransformDynamics dynamics);
    }

    public static class CollisionValueProviderExtensions
    {
        public static int GetIntValue(this CollisionValueProvider<float> provider, Collision collision, TransformDynamics dynamics)
        {
            return Mathf.RoundToInt(provider.GetValue(collision, dynamics));
        }
        
        public static int GetIntValue(this CollisionValueProvider<float> provider, Collision collision, TransformDynamics dynamics, float min, float max)
        {
            return Mathf.RoundToInt(Mathf.Clamp(provider.GetValue(collision, dynamics), min, max));
        }
    }
}