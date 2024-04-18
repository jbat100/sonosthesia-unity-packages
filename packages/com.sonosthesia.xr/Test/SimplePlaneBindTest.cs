using Sonosthesia.Utils.Editor;
using UnityEngine;

namespace Sonosthesia.XR.Test
{
    public class SimplePlaneBindTest : MonoBehaviour
    {
        [SerializeField] private Vector3 _target;

        private const float RADIUS = 0.02f; 
        
        protected void OnDrawGizmos()
        {
            Vector3 test = _target;

            GizmoUtils.DrawPlane(transform.position, transform.rotation, Color.cyan, Vector2.one, 0.001f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_target, RADIUS);

            if (GeomUtils.BindTargetToPlane(transform.position, transform.up, ref test))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(test, RADIUS);
            }
        }
    }
}