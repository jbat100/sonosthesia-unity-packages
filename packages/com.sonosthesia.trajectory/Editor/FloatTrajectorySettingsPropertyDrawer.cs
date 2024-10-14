using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Trajectory.Editor
{
    [CustomPropertyDrawer(typeof(FloatTrajectorySettings))]
    public class FloatTrajectorySettingsPropertyDrawer : TrajectorySettingsPropertyDrawer<float>
    {
        // note : tried inheritance but calling visibility update required instance variable 
        // or a lot of code, the cost was greater than the benefit
        
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
            
            SerializedProperty gridProp = property.FindPropertyRelative("_grid");
            PropertyField gridField = new PropertyField(gridProp);
            root.Add(gridField);
            
            SerializedProperty gridSizeProp = property.FindPropertyRelative("_gridSize");
            PropertyField gridSizeField = new PropertyField(gridSizeProp);
            root.Add(gridSizeField);
            
            SerializedProperty gridJumpProp = property.FindPropertyRelative("_gridJump");
            PropertyField gridJumpField = new PropertyField(gridJumpProp);
            root.Add(gridJumpField);
            
            SerializedProperty gridSnapProp = property.FindPropertyRelative("_gridSnap");
            PropertyField gridSnapField = new PropertyField(gridSnapProp);
            root.Add(gridSnapField);
            
            void UpdateVisibility()
            {
                // Debug.Log($"{this} {nameof(UpdateVisibility)}");
                
                TrajectoryType type = (TrajectoryType)trajectoryTypeProp.enumValueIndex;
                bool grid = gridProp.boolValue;
                
                easeTypeField.Show(type is TrajectoryType.Easing or TrajectoryType.Pulse);
                durationField.Show(type is TrajectoryType.Easing or TrajectoryType.Bounded or TrajectoryType.Pulse);
                positionField.Show(type is TrajectoryType.Bounded or TrajectoryType.Immediate);
                velocityField.Show(type is not TrajectoryType.None);
                
                gridField.Show(type is TrajectoryType.Bounded);
                gridSizeField.Show(type is TrajectoryType.Bounded && grid);
                gridJumpField.Show(type is TrajectoryType.Bounded && grid);
                gridSnapField.Show(type is TrajectoryType.Bounded && grid);
            }

            UpdateVisibility();

            trajectoryTypeField.RegisterValueChangeCallback(_ => UpdateVisibility());
            gridField.RegisterValueChangeCallback(_ => UpdateVisibility());
            
            return root;
        }
    }
}