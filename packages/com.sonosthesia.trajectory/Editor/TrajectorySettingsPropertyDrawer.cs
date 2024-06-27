using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Trajectory.Editor
{
    public class TrajectorySettingsPropertyDrawer<T> : PropertyDrawer where T : struct
    {
        protected StyleEnum<DisplayStyle> Show(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            // Create property fields for each field in the settings class
            SerializedProperty trajectoryTypeProp = property.FindPropertyRelative("_trajectoryType");
            PropertyField trajectoryTypeField = new PropertyField(trajectoryTypeProp);
            root.Add(trajectoryTypeField);

            SerializedProperty easeTypeProp = property.FindPropertyRelative("_easeType");
            PropertyField easeTypeField = new PropertyField(easeTypeProp);
            root.Add(easeTypeField);

            SerializedProperty durationProp = property.FindPropertyRelative("_duration");
            PropertyField durationField = new PropertyField(durationProp);
            root.Add(durationField);

            SerializedProperty positionProp = property.FindPropertyRelative("_position");
            PropertyField positionField = new PropertyField(positionProp);
            root.Add(positionField);
            
            SerializedProperty velocityProp = property.FindPropertyRelative("_velocity");
            PropertyField velocityField = new PropertyField(velocityProp);
            root.Add(velocityField);
            
            // Method to update the visibility of fields based on the enum value
            void UpdateVisibility()
            {
                Debug.Log($"{this} {nameof(UpdateVisibility)}");
                TrajectoryType type = (TrajectoryType)trajectoryTypeProp.enumValueIndex;
                easeTypeField.style.display = Show(type is TrajectoryType.Easing);
                durationField.style.display = Show(type is TrajectoryType.Easing or TrajectoryType.Bounded);
                positionField.style.display = Show(type is TrajectoryType.Bounded or TrajectoryType.Immediate);
                velocityField.style.display = Show(type is TrajectoryType.Easing or TrajectoryType.Bounded or TrajectoryType.Immediate);
            }

            // Initial visibility update
            UpdateVisibility();

            trajectoryTypeField.RegisterValueChangeCallback(evt => UpdateVisibility());
            
            return root;
        }
    }
}