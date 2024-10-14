using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class VisualElementExtensions
    {
        public static void Show(this VisualElement element, bool show)
        {
            element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        public static bool TryGetElementByName<T>(this VisualElement visualElement, string name, out T element) where T : VisualElement
        {
            element = visualElement.Q<T>(name);
            if (element != null)
            {
                return true;
            }
            Debug.LogError($"Expected successful query for type {typeof(T).Name} with name {name}");
            return false;
        }


        public static void AddSpace(this VisualElement visualElement) => visualElement.AddSpace(10);
        
        public static void AddSpace(this VisualElement visualElement, StyleLength styleLength)
        {
            // Create a spacer element
            VisualElement spacer = new VisualElement();
            spacer.style.height = styleLength; 
            visualElement.Add(spacer);
        }
        
        public static void AddSeparator(this VisualElement visualElement)
        {
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.marginTop = 10;
            separator.style.marginBottom = 10;
            separator.style.backgroundColor = Color.gray;
            visualElement.Add(separator);
        }
        
        public static void AddField(this VisualElement visualElement, SerializedObject serializedObject, string name)
        {
            visualElement.AddField(serializedObject, name, out _, out _);
        }

        public static void AddField(this VisualElement visualElement, SerializedObject serializedObject, string name, out SerializedProperty serializedProperty, out PropertyField propertyField)
        {
            serializedProperty = serializedObject.FindProperty(name);
            propertyField = new PropertyField(serializedProperty);
            visualElement.Add(propertyField);
        }
        
        public static void AddRelativeField(this VisualElement visualElement, SerializedProperty serializedProperty, string name)
        {
            visualElement.AddRelativeField(serializedProperty, name, out _, out _);
        }
        
        public static void AddRelativeField(this VisualElement visualElement, SerializedProperty serializedProperty, string name, 
            out SerializedProperty relativeProperty, out PropertyField propertyField)
        {
            relativeProperty = serializedProperty.FindPropertyRelative(name);
            propertyField = new PropertyField(relativeProperty);
            visualElement.Add(propertyField);
        }
    }
}