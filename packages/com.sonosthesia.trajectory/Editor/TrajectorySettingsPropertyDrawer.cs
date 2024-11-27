using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Trajectory.Editor
{
    public class TrajectorySettingsPropertyDrawer<T> : PropertyDrawer where T : struct
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            root.AddRelativeField(property, "_trajectoryType", out SerializedProperty trajectoryTypeProp, out PropertyField trajectoryTypeField);
            root.AddRelativeField(property, "_easeType", out _, out PropertyField easeTypeField);
            root.AddRelativeField(property, "_duration", out _, out PropertyField durationField);
            root.AddRelativeField(property, "_position", out _, out PropertyField positionField);
            root.AddRelativeField(property, "_velocity", out _, out PropertyField velocityField);

            void UpdateVisibility()
            {
                // Debug.Log($"{this} {nameof(UpdateVisibility)}");
                TrajectoryType type = (TrajectoryType)trajectoryTypeProp.enumValueIndex;
                easeTypeField.Show(type is TrajectoryType.Easing or TrajectoryType.Pulse);
                durationField.Show(type is TrajectoryType.Easing or TrajectoryType.Bounded or TrajectoryType.Pulse);
                positionField.Show(type is TrajectoryType.Bounded or TrajectoryType.Immediate);
                velocityField.Show(type is not TrajectoryType.None);
            }

            UpdateVisibility();

            trajectoryTypeField.RegisterValueChangeCallback(evt => UpdateVisibility());
            
            return root;
        }
    }
}