using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Target.Editor
{
    public class MaterialTargetEditor<T> : UnityEditor.Editor where T : struct
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.AddField(serializedObject, "_source");
            root.AddField(serializedObject, "_distinct");
            root.AddField(serializedObject, "_processingFactory");
            
            root.AddSeparator();
            
            root.AddField(serializedObject, "_name");
            root.AddField(serializedObject, "_renderer");
            root.AddField(serializedObject, "_renderers");
            root.AddField(serializedObject, "_group");
            
            root.AddSeparator();
            
            root.AddField(serializedObject, "_usePropertyBlock", out SerializedProperty usePropertyBlockProperty, out PropertyField usePropertyBlockField);
            root.AddField(serializedObject, "_materialSelector", out SerializedProperty materialSelectorProperty, out PropertyField materialSelectorField);
            root.AddField(serializedObject, "_materialIndex", out SerializedProperty materialIndexProperty, out PropertyField materialIndexField);

            root.UpdateVisibility(UpdateVisibility, usePropertyBlockProperty, materialSelectorProperty);
            
            return root;

            void UpdateVisibility()
            {
                bool usePropertyBlock = usePropertyBlockProperty.boolValue;
                MaterialSelector selector = (MaterialSelector)materialSelectorProperty.enumValueIndex;
                materialSelectorField.Show(!usePropertyBlock);
                materialIndexField.Show(!usePropertyBlock && selector is MaterialSelector.Indexed);
            }
        }
    }
}