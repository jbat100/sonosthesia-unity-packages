using UnityEditor;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Application.Editor
{
    [CustomEditor(typeof(SceneSwitcherTest))]
    public class SceneSwitcherTestEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.AddField(serializedObject, "_switcher");
            root.AddField(serializedObject, "_scenes");
            
            root.AddSeparator();
            
            SceneSwitcherTest test = (SceneSwitcherTest)target;
            
            Button trigger = new Button(() => test.Switch()) { text = "Switch" };
            
            root.Add(trigger);
            
            return root;
        }
    }
}