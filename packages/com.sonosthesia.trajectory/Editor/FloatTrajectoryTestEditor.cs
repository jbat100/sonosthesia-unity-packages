using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Trajectory.Editor
{
    [CustomEditor(typeof(FloatTrajectoryTest))]
    public class FloatTrajectoryTestEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var settingsProperty = serializedObject.FindProperty("_settings");
            var settingsField = new PropertyField(settingsProperty);
            root.Add(settingsField);

            FloatTrajectoryTest test = (FloatTrajectoryTest)target;

            // Add a separator
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.marginTop = 10;
            separator.style.marginBottom = 10;
            separator.style.backgroundColor = Color.gray;
            root.Add(separator);
            
            var trigger = new Button(() => test.Trigger()) { text = "Trigger" };
            var stop = new Button(() => test.Stop()) { text = "Stop" };
            
            trigger.style.flexGrow = 1;
            stop.style.flexGrow = 1;

            // Create a container for the buttons with a horizontal layout
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            
            buttonContainer.Add(trigger);
            buttonContainer.Add(stop);
            
            root.Add(buttonContainer);

            return root;
        }
    }
}