using UnityEngine;

namespace Sonosthesia.Scaffold
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BaseGroupInstantiator), true)]
    public class BaseGroupInstantiatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BaseGroupInstantiator intantiator = (BaseGroupInstantiator)target;
            if(GUILayout.Button("Reload"))
            {
                intantiator.Reload();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
#endif
    
    public abstract class BaseGroupInstantiator : MonoBehaviour
    {
        public abstract void Reload();
    }
}