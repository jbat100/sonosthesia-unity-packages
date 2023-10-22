using UnityEngine;

namespace Sonosthesia.Instrument
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
            }
        }
    }
#endif
    
    public abstract class BaseGroupInstantiator : MonoBehaviour
    {
        public abstract void Reload();
    }
}