using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(FloatModulationSettings))]
    public class FloatModulationSettingsDrawer : HorizontalPropertyDrawer
    {
        protected override void CreateContent(SerializedProperty property, VisualElement container)
        {
            SerializedProperty strategyProp = property.FindPropertyRelative("_strategy");
            SerializedProperty scaleProp = property.FindPropertyRelative("_scale");
            SerializedProperty offsetProp = property.FindPropertyRelative("_offset");

            EnumField strategyField = UIHorizontalUtils.CreateEnumField(strategyProp, 40);
            Label scaleLabel = UIHorizontalUtils.CreateLabel("Scale", 40);
            FloatField scaleField = UIHorizontalUtils.CreateFloatField(scaleProp);
            Label offsetLabel = UIHorizontalUtils.CreateLabel("Offset", 40);
            FloatField offsetField = UIHorizontalUtils.CreateFloatField(offsetProp);
            
            container.Add(strategyField);
            container.Add(scaleLabel);
            container.Add(scaleField);
            container.Add(offsetLabel);
            container.Add(offsetField);
        }
    }
}