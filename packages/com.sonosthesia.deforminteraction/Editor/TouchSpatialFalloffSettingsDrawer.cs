using Sonosthesia.Deform;
using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.DeformInteraction.Editor
{
    [CustomPropertyDrawer(typeof(TouchSpatialFalloffSettings))]
    public class TouchSpatialFalloffSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            root.Add(UIElementUtils.Separator());
            
            root.Add(UIElementUtils.SectionLabel(property.name.PropertyNameToLabel()));

            root.AddRelativeField(property, "_active", 
                out SerializedProperty activeProp, out PropertyField activeField);
            
            root.AddRelativeField(property, "_easeType", 
                out SerializedProperty _, out PropertyField easeTypeField);

            root.AddRelativeField(property, "_center", 
                out SerializedProperty _, out PropertyField centerField);
            
            root.AddRelativeField(property, "_shape", 
                out SerializedProperty shapeProp, out PropertyField shapeField);
            
            root.AddRelativeField(property, "_space", 
                out SerializedProperty _, out PropertyField spaceField);
            
            root.AddRelativeField(property, "_direction", 
                out SerializedProperty _, out PropertyField directionField);

            void UpdateVisibility()
            {
                bool active = activeProp.boolValue;

                if (active)
                {
                    easeTypeField.Show(true);
                    centerField.Show(true);
                    shapeField.Show(true);
                    
                    SpatialFalloffShape shape = (SpatialFalloffShape)shapeProp.enumValueIndex;
                    
                    spaceField.Show(shape is not SpatialFalloffShape.Spherical);
                    directionField.Show(shape is not SpatialFalloffShape.Spherical);
                }
                else
                {
                    easeTypeField.Show(false);
                    centerField.Show(false);
                    shapeField.Show(false);
                    spaceField.Show(false);
                    directionField.Show(false);
                }
            }

            UpdateVisibility();

            activeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            shapeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}