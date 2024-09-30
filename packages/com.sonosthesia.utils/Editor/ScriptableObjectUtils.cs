using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
    // https://docs.unity3d.com/ScriptReference/Object.FindObjectsOfType.html
    // https://discussions.unity.com/t/how-can-i-find-all-gameobjects-that-directly-reference-a-scriptableobject/911091/2
    
    public class ScriptableObjectUtils
    {
        private readonly Dictionary<Type, List<FieldInfo>> assignableFieldsByComponentType 
            = new Dictionary<Type, List<FieldInfo>>() { { typeof(MonoBehaviour), null } };

        public IEnumerable<GameObject> FindAllGameObjectsThatDirectlyReference(ScriptableObject target)
        {
            HashSet<GameObject> results = new HashSet<GameObject>();

            foreach(var component in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true))
            {
                if(results.Contains(component.gameObject))
                {
                    continue;
                }

                Type componentType = component.GetType();
                if(!assignableFieldsByComponentType.TryGetValue(componentType, out var assignableFields))
                {
                    var type = componentType;
                    do
                    {
                        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                        foreach(var field in type.GetFields(flags))
                        {
                            if(field.FieldType.IsInstanceOfType(target))
                            {
                                assignableFields ??= new List<FieldInfo>();
                                assignableFields.Add(field);
                            }
                        }

                        type = type.BaseType;
                        if(!assignableFieldsByComponentType.TryGetValue(type, out var assignableFieldsFromBaseTypes))
                        {
                            continue;
                        }

                        if(assignableFieldsFromBaseTypes is null)
                        {
                            break;
                        }

                        assignableFields.AddRange(assignableFieldsFromBaseTypes);
                        break;
                    }
                    while(true);

                    assignableFieldsByComponentType.Add(componentType, assignableFields);
                }

                if(assignableFields is null)
                {
                    continue;
                }

                foreach(var field in assignableFields)
                {
                    if(field.GetValue(component) as UnityEngine.Object == target)
                    {
                        results.Add(component.gameObject);
                        break;
                    }
                }
            }

            return results;
        }
    }
}