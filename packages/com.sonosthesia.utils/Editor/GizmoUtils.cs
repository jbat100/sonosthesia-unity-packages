using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
    public static class GizmoUtils
    {
        public static void DrawPlane(Vector3 position, Quaternion rotation, Color color, Vector2 dimensions, float thickness)
        {
            // https://discussions.unity.com/t/gizmos-and-drawing-a-plane-with-rotation/63167
            
            Matrix4x4 trs = Matrix4x4.TRS(position, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, new Vector3(dimensions.x, thickness, dimensions.y));
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;    
        }
    }
}