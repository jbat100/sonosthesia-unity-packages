using UnityEditor;
using UnityEngine.UIElements;
using Sonosthesia.Utils.Editor;

namespace Sonosthesia.Application.Editor
{
    [CustomEditor(typeof(DemoUIWindowTest))]
    public class DemoUIWindowTestEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.AddField(serializedObject, "_intent");
            root.AddField(serializedObject, "_currentScene");
            
            root.AddSeparator();
            
            DemoUIWindowTest test = (DemoUIWindowTest)target;

            void AddIntentButton(string key)
            {
                Button trigger = new Button(() => test.Broadcast(key)) { text = key };
                root.Add(trigger);
            }
            
            AddIntentButton("EMPTY");
            AddIntentButton(UIDemoWindowIntentKeys.MAIN);
            AddIntentButton(UIDemoWindowIntentKeys.SCENE);
            AddIntentButton(UIDemoWindowIntentKeys.PERFORMANCE);
            
            Button trigger = new Button(() => test.AdaptiveBroadcast()) { text = "ADAPTIVE" };
            root.Add(trigger);

            return root;
        }
    }
}