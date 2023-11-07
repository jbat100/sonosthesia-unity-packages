using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Builder
{
    [CustomEditor(typeof(PerlinGrapher))]
    public class PerlinGrapherEditor : Editor
    {
        protected void OnSceneGUI()
        {
            PerlinGrapher grapher = (PerlinGrapher) target;
            if (grapher == null)
            {
                return;
            }
            
            Handles.color = Color.blue;
            Handles.Label(grapher.LineRenderer.GetPosition(0) + Vector3.up * 2, "Layer : " + grapher.gameObject.name);
        }
    }
}