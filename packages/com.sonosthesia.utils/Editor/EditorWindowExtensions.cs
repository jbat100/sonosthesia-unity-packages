using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class EditorWindowExtensions
    {
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
    }
}