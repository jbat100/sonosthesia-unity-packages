using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Interaction.Editor
{
    [CustomPropertyDrawer(typeof(DynamicTrackingSettings))]
    public class DynamicTrackingSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_strategy", 
                out SerializedProperty strategyProp, out PropertyField strategyField);
            
            root.AddRelativeField(property, "_drag", 
                out SerializedProperty _, out PropertyField dragField);

            void UpdateVisibility()
            {
                DynamicTrackingStrategy strategy = (DynamicTrackingStrategy)strategyProp.enumValueIndex;
                dragField.Show(strategy is DynamicTrackingStrategy.FreezeVelocity);
            }

            root.UpdateVisibility(UpdateVisibility, strategyProp);
            
            return root;
        }
    }
}