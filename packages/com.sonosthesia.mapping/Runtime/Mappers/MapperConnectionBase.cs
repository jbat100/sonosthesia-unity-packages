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
                foreach (Transform child in connection.transform)
                {
                    if (!connection.HasSlot(child.name))
                    {
                        connection.CreateSlot(child.name);
                    }
                }
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
        public abstract bool HasSlot(string slotName);
        
        public abstract void CreateSlot(string slotName);
        
        public abstract void DeleteSlot(string slotName);
        
        public abstract void DeleteAllSlots();
    }
}