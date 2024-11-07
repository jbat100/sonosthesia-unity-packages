using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    [CustomPropertyDrawer(typeof(OneEuroFilterSettings))]
    public class OneEuroFilterSettingsDrawer : HorizontalPropertyDrawer
    {
        protected override void CreateContent(SerializedProperty property, VisualElement container)
        {
            SerializedProperty betaProp = property.FindPropertyRelative("_beta");
            SerializedProperty minCutoffProp = property.FindPropertyRelative("_minCutoff");

            Label betaLabel = UIHorizontalUtils.CreateLabel("Beta", 30);
            FloatField betaField = UIHorizontalUtils.CreateFloatField(betaProp);
            Label minCutoffLabel = UIHorizontalUtils.CreateLabel("Min Cutoff", 70);
            FloatField minCutoffField = UIHorizontalUtils.CreateFloatField(minCutoffProp);
            
            container.Add(betaLabel);
            container.Add(betaField);
            container.Add(minCutoffLabel);
            container.Add(minCutoffField);
        }
    }
}