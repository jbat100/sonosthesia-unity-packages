using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class TransformExtensions
    {
        public static void ComponentScan<T>(this Transform root, bool recursive, Func<string, bool> check, Action<string, T> add) where T : Component
        {
            foreach (Transform child in root)
            {
                if (check == null || check(child.name))
                {
                    T component = child.GetComponent<T>();
                    if (component && add != null)
                    {
                        add(child.name, component);
                    }
                }
                if (recursive)
                {
                    child.ComponentScan(true, check, add);
                }
            }
        }
    }
}