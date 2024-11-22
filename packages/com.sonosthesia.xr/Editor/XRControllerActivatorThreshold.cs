using UnityEditor;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.XR.Editor
{
    [CustomPropertyDrawer(typeof(XRControllerActivator.Threshold))]
    public class XRControllerActivatorThreshold : HorizontalPropertyDrawer
    {
        protected override void CreateContent(SerializedProperty property, VisualElement container)
        {
            SerializedProperty thresholdProp = property.FindPropertyRelative("_threshold");
            SerializedProperty valueProp = property.FindPropertyRelative("_value");

            EnumField easeTypeField = UIHorizontalUtils.CreateEnumField(thresholdProp, 125);
            FloatField durationField = UIHorizontalUtils.CreateFloatField(valueProp);

            container.Add(easeTypeField);
            container.Add(durationField);
        }
    }
}