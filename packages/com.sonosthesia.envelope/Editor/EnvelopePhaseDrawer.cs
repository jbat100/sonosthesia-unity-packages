using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Envelope.Editor
{
    [CustomPropertyDrawer(typeof(EnvelopePhase))]
    public class EnvelopePhaseDrawer : HorizontalPropertyDrawer
    {
        protected override VisualElement CreateContent(SerializedProperty property)
        {
            VisualElement container = UIHorizontalUtils.CreatePropertyContainer();
            
            SerializedProperty easeTypeProp = property.FindPropertyRelative("_easeType");
            SerializedProperty durationProp = property.FindPropertyRelative("_duration");

            EnumField easeTypeField = UIHorizontalUtils.CreateEnumField(easeTypeProp, 125);

            Label durationLabel = UIHorizontalUtils.CreateLabel("sec", 25);
            FloatField durationField = UIHorizontalUtils.CreateFloatField(durationProp);

            container.Add(easeTypeField);
            container.Add(durationField);
            container.Add(durationLabel);

            return container;
        }
    }
}