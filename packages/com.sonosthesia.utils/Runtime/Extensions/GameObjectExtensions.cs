using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this MonoBehaviour monoBehaviour) where T : MonoBehaviour
        {
            T result = monoBehaviour.GetComponent<T>();
            if (!result)
            {
                result = monoBehaviour.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static RigidTransform ToRigidTransform(this Transform transform)
        {
            return new RigidTransform(transform.rotation, transform.position);
        }

        public static void SafeSetActive(this GameObject target, bool active)
        {
            if (!target || target.activeSelf == active)
            {
                return;
            }
            target.SetActive(active);
        }
    }    
}


