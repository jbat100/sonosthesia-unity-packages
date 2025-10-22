using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(PrefabSelectorSettings<>))]
    public class PrefabSelectorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_selectorType", 
                out SerializedProperty selectorTypeProp, out PropertyField selectorTypeField);
            
            root.AddRelativeField(property, "_prefab", 
                out SerializedProperty _, out PropertyField prefabField);
            
            root.AddRelativeField(property, "_multiSelection", 
                out SerializedProperty _, out PropertyField multiSelectionField);

            root.AddRelativeField(property, "_prefabs", 
                out SerializedProperty _, out PropertyField prefabsField);

            root.UpdateVisibility(UpdateVisibility, selectorTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                PrefabSelectorType selectorType = (PrefabSelectorType)selectorTypeProp.enumValueIndex;

                prefabField.Show(selectorType is PrefabSelectorType.Single);
                multiSelectionField.Show(selectorType is PrefabSelectorType.Multi);
                prefabsField.Show(selectorType is PrefabSelectorType.Multi);
            }
        }
    }
}