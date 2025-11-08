using Sonosthesia.Utils;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Extractor.Editor
{
    // drawer for simple StaticExtractorSettings which only add an _extractorType field which specifies 
    // at least custom and constant cases as enumValueIndex
    
    public abstract class BaseFloatStaticExtractorSettingsDrawer : PropertyDrawer
    {
        protected abstract int ConstantIndex { get; }
        protected abstract int CustomIndex { get; }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            Label titleLabel = UIElementUtils.TitleLabel(property.name.PropertyNameToLabel());
            root.Add(titleLabel);

            root.AddRelativeField(property, "_extractorType",
                out SerializedProperty extractorTypeProp, out PropertyField _);

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
                extractorField.Show(extractorTypeProp.enumValueIndex == CustomIndex);
                constantValueField.Show(extractorTypeProp.enumValueIndex == ConstantIndex);
            }
        }
    }
    
    [CustomPropertyDrawer(typeof(PeakFloatStaticExtractorSettings))]
    public class PeakFloatStaticExtractorSettingsDrawer : BaseFloatStaticExtractorSettingsDrawer
    {
        protected override int ConstantIndex => (int)PeakExtractorType.Constant;
        protected override int CustomIndex => (int)PeakExtractorType.Custom;
    }
}