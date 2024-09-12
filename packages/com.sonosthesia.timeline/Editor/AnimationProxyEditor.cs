using System;
using Sonosthesia.Utils.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Globalization;
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

            Signal<float> CreateSignal(FieldInfo field, Transform parent)
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
                        if (proxy.signal)
                        {
                            continue;
                        }
                        proxy.signal = CreateSignal(field, parent);
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
                        if (!proxy.signal)
                        {
                            continue;
                        }
                        DestroyImmediate(proxy.signal.gameObject);
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
    }
}