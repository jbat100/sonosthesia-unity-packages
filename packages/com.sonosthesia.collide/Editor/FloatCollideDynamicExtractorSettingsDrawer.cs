using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Collide.Editor
{
    [CustomPropertyDrawer(typeof(FloatCollideDynamicExtractorSettings))]
    public class FloatCollideDynamicExtractorSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);

            root.AddRelativeField(property, "_extractorType",
                out SerializedProperty extractorTypeProp, out PropertyField _);

            root.AddRelativeField(property, "_followStrategy",
                out SerializedProperty _, out PropertyField followStrategyField, false);

            root.AddRelativeField(property, "_extractor",
                out SerializedProperty _, out PropertyField extractorField, false);

            root.AddRelativeField(property, "_constantValue",
                out SerializedProperty _, out PropertyField constantValueField, false);

            root.AddRelativeField(property, "_velocityType",
                out SerializedProperty _, out PropertyField velocityTypeField, false);

            root.AddRelativeField(property, "_axes",
                out SerializedProperty _, out PropertyField axesField, false);

            root.AddRelativeField(property, "_postProcessing",
                out SerializedProperty _, out PropertyField _);

            root.UpdateVisibility(UpdateVisibility, extractorTypeProp);

            return root;

            void UpdateVisibility()
            {
                FloatCollideDynamicExtractorSettings.ExtractorType extractorType =
                    (FloatCollideDynamicExtractorSettings.ExtractorType)extractorTypeProp.enumValueIndex;

                followStrategyField.Show(extractorType is not FloatCollideDynamicExtractorSettings.ExtractorType.Constant);
                extractorField.Show(extractorType is FloatCollideDynamicExtractorSettings.ExtractorType.Custom);
                constantValueField.Show(extractorType is FloatCollideDynamicExtractorSettings.ExtractorType.Constant);
                velocityTypeField.Show(extractorType is FloatCollideDynamicExtractorSettings.ExtractorType.Velocity);
                axesField.Show(extractorType is FloatCollideDynamicExtractorSettings.ExtractorType.Distance);
            }
        }
    }
}