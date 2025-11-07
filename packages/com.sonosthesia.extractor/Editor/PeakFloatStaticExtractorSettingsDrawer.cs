using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Extractor.Editor
{
    [CustomPropertyDrawer(typeof(PeakFloatStaticExtractorSettings))]
    public class PeakFloatStaticExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);

            root.AddRelativeField(property, "_extractorType",
                out SerializedProperty extractorTypeProp, out PropertyField extractorTypeField);

            root.AddRelativeField(property, "_extractor",
                out SerializedProperty _, out PropertyField extractorField);

            root.AddRelativeField(property, "_constantValue",
                out SerializedProperty _, out PropertyField constantValueField);

            root.AddRelativeField(property, "_processor",
                out SerializedProperty _, out PropertyField _);
            
            
            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);

            return root;

            void UpdateVisibility()
            {
                PeakExtractorType extractorType = (PeakExtractorType)extractorTypeProp.enumValueIndex;
                extractorField.Show(extractorType is PeakExtractorType.Custom);
                constantValueField.Show(extractorType is PeakExtractorType.Constant);
            }
        }
    }
}