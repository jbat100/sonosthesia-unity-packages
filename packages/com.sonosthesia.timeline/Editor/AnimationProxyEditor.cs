using System;
using System.Collections.Generic;
using Sonosthesia.Utils.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline.Editor
{
    [CustomEditor(typeof(AnimationProxy), true)]
    public class AnimationProxyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            AnimationProxy proxy = (AnimationProxy)target;
            if (GUILayout.Button("Create Signals"))
            {
                Perform(proxy, "Create Signals", () => CreateSignals(proxy));
            }
            if (GUILayout.Button("Clear Signals"))
            {
                Perform(proxy, "Clear Signals", () => ClearSignals(proxy, false));
            }
            if (GUILayout.Button("Clear All"))
            {
                Perform(proxy, "Clear All", () => ClearSignals(proxy, true));
            }
            if (GUILayout.Button("Auto Fill"))
            {
                Perform(proxy, "Auto Fill", () => Autofill(proxy));
            }
            
            GUILayoutUtils.DrawSeparator(); 
            
            DrawDefaultInspector();
        }

        private void Perform(AnimationProxy proxy, string actionName, Action action)
        {
            Undo.RecordObject(proxy, " AnimationProxy " + actionName);
            action();
            PrefabUtility.RecordPrefabInstancePropertyModifications(proxy);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        
         private static string ConvertToPascalCase(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return fieldName;
            }

            if (fieldName.StartsWith("_"))
            {
                fieldName = fieldName.Substring(1);
            }
            else if (fieldName.StartsWith("m_"))
            {
                fieldName = fieldName.Substring(2);
            }

            if (fieldName.Length > 0)
            {
                fieldName = char.ToUpper(fieldName[0], CultureInfo.InvariantCulture) + fieldName.Substring(1);
            }

            return fieldName;
        }

         private void CreateProxySignals(object obj, Transform parent)
        {
            Type type = obj.GetType();

            FloatSignal CreateSignal(FieldInfo field, Transform parent)
            {
                string fieldName = ConvertToPascalCase(field.Name);
                GameObject child = new GameObject(fieldName);
                child.transform.parent = parent;
                return child.AddComponent<FloatSignal>();
            }

            while (type != null && type != typeof(MonoBehaviour))
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(AnimationProxy.Proxy))
                    {
                        // Proxy is a struct which makes things a little harder, but no choice
                        AnimationProxy.Proxy proxy = (AnimationProxy.Proxy)field.GetValue(obj);
                        if (proxy.signal.Value != null)
                        {
                            continue;
                        }
                        proxy.signal.Value = CreateSignal(field, parent);
                        field.SetValue(obj, proxy);
                    }
                    else if (typeof(IProxyContainer).IsAssignableFrom(field.FieldType))
                    {
                        string containerName = ConvertToPascalCase(field.Name);
                        object container = field.GetValue(obj);
                        GameObject child = new GameObject(containerName);
                        child.transform.parent = parent;
                        CreateProxySignals(container, child.transform);
                    }
                }
                type = type.BaseType;
            }
        }
        
        private void CreateSignals(AnimationProxy proxy)
        {
            CreateProxySignals(proxy, proxy.transform);
        }

        private void ClearProxySignals(object obj, Transform parent)
        {
            if (obj == null)
            {
                return;
            }
            
            Type type = obj.GetType();

            while (type != null && type != typeof(MonoBehaviour))
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(AnimationProxy.Proxy))
                    {
                        // Proxy is a struct which makes things a little harder, but no choice
                        AnimationProxy.Proxy proxy = (AnimationProxy.Proxy)field.GetValue(obj);
                        MonoBehaviour signalBehaviour = proxy.signal.Value as MonoBehaviour;
                        if (!signalBehaviour)
                        {
                            continue;
                        }
                        DestroyImmediate(signalBehaviour.gameObject);
                        proxy.signal = null;
                        field.SetValue(obj, proxy);
                    }
                    else if (typeof(IProxyContainer).IsAssignableFrom(field.FieldType))
                    {
                        string containerName = ConvertToPascalCase(field.Name);
                        object container = field.GetValue(obj);
                        GameObject child = new GameObject(containerName);
                        child.transform.parent = parent;
                        ClearProxySignals(container, child.transform);
                    }
                }
                type = type.BaseType;
            }
        }
        
        private void ClearSignals(AnimationProxy proxy, bool all)
        {
            ClearProxySignals(proxy, proxy.transform);

            if (!all)
            {
                return;
            }
            
            Transform transform = proxy.transform;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }

        private static void Autofill(AnimationProxy proxy)
        {
            if (!proxy)
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(proxy, "Auto Assign Signals");

            var so = new SerializedObject(proxy);

            // Discover all Proxy field paths according to the type structure:
            // - Proxy fields
            // - IProxyContainer fields (recursively)
            List<string> proxyPaths = new ();
            DiscoverProxyFieldPaths(proxy.GetType(), string.Empty, proxyPaths, new HashSet<Type>());

            foreach (string proxyPath in proxyPaths)
            {
                TryAssignSignalForProxyPath(so, proxy.transform, proxyPath);
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(proxy);
        }

        private static void DiscoverProxyFieldPaths(
            Type type,
            string prefix,
            List<string> result,
            HashSet<Type> visitedTypes)
        {
            if (type == null)
            {
                return;
            }

            if (!visitedTypes.Add(type))
            {
                return;
            }

            // Walk up inheritance chain (like your runtime code)
            while (type != null && type != typeof(MonoBehaviour))
            {
                FieldInfo[] fieldInfos = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fieldInfos)
                {
                    string fieldPath = string.IsNullOrEmpty(prefix)
                        ? field.Name
                        : $"{prefix}.{field.Name}";

                    if (field.FieldType == typeof(AnimationProxy.Proxy))
                    {
                        result.Add(fieldPath);
                    }
                    else if (typeof(IProxyContainer).IsAssignableFrom(field.FieldType))
                    {
                        DiscoverProxyFieldPaths(field.FieldType, fieldPath, result, visitedTypes);
                    }
                }

                type = type.BaseType;
            }
        }
        
        private static void TryAssignSignalForProxyPath(SerializedObject so, Transform root, string proxyPath)
        {
            if (so == null || !root || string.IsNullOrEmpty(proxyPath))
            {
                return;
            }

            // The serialized field inside Proxy is assumed to be called "signal"
            string fullSignalPath = $"{proxyPath}.signal";

            SerializedProperty signalProperty = so.FindProperty(fullSignalPath);
            if (signalProperty == null)
            {
                return;
            }

            string gameObjectPath = $"{proxyPath}.Animation";
            
            // Map "sphere1.spin" to transform hierarchy sphere1/spin
            string[] segments = gameObjectPath.Split('.');
            Transform current = root;

            foreach (string segment in segments)
            {
                Transform child = FindDirectChildByName(current, segment);
                if (!child)
                {
                    // No matching hierarchy -> abandon this Proxy
                    return;
                }

                current = child;
            }

            // Find a component implementing ISignal<float> on the final Transform
            ISignal<float> foundSignal = current.GetComponent<ISignal<float>>();
            if (foundSignal == null)
            {
                return;
            }

            // Assign into the InterfaceReference<ISignal<float>> backing field
            SerializedProperty backingObjectProp = signalProperty.FindPropertyRelative("underlyingObject");
            if (backingObjectProp != null)
            {
                backingObjectProp.objectReferenceValue = foundSignal as UnityEngine.Object;
            }
        }

        private static Transform FindDirectChildByName(Transform parent, string name)
        {
            if (!parent)
            {
                return null;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (string.Compare(child.name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return child;
                }
            }

            return null;
        }

    }
}