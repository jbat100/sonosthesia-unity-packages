using UnityEngine;

namespace Sonosthesia.Mapping
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MapperConnectionBase), true)]
    public class MapperConnectionBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MapperConnectionBase connection = (MapperConnectionBase)target;
            if (GUILayout.Button("Autofill"))
            {
                connection.AutofillSlots();
            }
            if (GUILayout.Button("Clear"))
            {
                connection.DeleteAllSlots();
            }
        }
    }
#endif
    
    public abstract class MapperConnectionBase : MonoBehaviour
    {
        public abstract void AutofillSlots();
        
        public abstract void DeleteAllSlots();
    }
}