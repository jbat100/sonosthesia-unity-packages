using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Envelope.Editor
{
    [CustomPropertyDrawer(typeof(EnvelopePhase))]
    public class EnvelopePhaseDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreateContainer(0);

            Label titleLabel = UIHorizontalUtils.CreatePropertyLabel(property.name);

            SerializedProperty easeTypeProp = property.FindPropertyRelative("_easeType");
            SerializedProperty durationProp = property.FindPropertyRelative("_duration");

            //Label easeTypeLabel = UIHorizontalUtils.CreateLabel("Type", 40);
            EnumField easeTypeField = UIHorizontalUtils.CreateEnumField(easeTypeProp, 125);

            Label durationLabel = UIHorizontalUtils.CreateLabel("sec", 50);
            FloatField durationField = UIHorizontalUtils.CreateFloatField(durationProp);

            container.Add(titleLabel);
            //container.Add(easeTypeLabel);
            container.Add(easeTypeField);
            container.Add(durationField);
            container.Add(durationLabel);

            return container;
        }
    }
}