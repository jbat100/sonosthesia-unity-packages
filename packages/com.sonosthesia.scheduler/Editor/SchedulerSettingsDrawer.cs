using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Scheduler.Editor
{
    [CustomPropertyDrawer(typeof(SchedulerSettings))]
    public class SchedulerSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);
            
            root.AddRelativeField(property, "_schedulerType", 
                out SerializedProperty schedulerTypeProp, out PropertyField schedulerTypeField);
            
            root.AddRelativeField(property, "_scheduler", 
                out SerializedProperty _, out PropertyField schedulerField);


            root.UpdateVisibility(UpdateVisibility, schedulerTypeProp);
            
            return root;

            void UpdateVisibility()
            {
                SchedulerSettings.SchedulerType schedulerType = (SchedulerSettings.SchedulerType)schedulerTypeProp.enumValueIndex;
                schedulerField.Show(schedulerType is SchedulerSettings.SchedulerType.Custom);
            }
        }
    }
}