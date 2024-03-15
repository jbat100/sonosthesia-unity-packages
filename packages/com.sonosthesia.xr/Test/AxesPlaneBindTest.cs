using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.XR.Test
{
    public class AxesPlaneBindTest : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] private Axes _axes;
        
        [SerializeField] private Transform _upper;

        [SerializeField] private Transform _lower;
        
        private const float RADIUS = 0.02f; 
        
        protected void OnDrawGizmos()
        {
            if (!_target || !_upper || !_lower)
            {
                return;
            }
            
            Vector3 targetPosition = _target.position;
            Quaternion targetRotation = _target.rotation;
            Vector3 upperPosition = _upper.position;
            Vector3 lowerPosition = _lower.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPosition, RADIUS);
            bool bound = false;

            void DrawBind(Vector3 position, Vector3 localNormal)
            {
                //Quaternion rotation = Quaternion.FromToRotation(localNormal, Vector3.up);
                //GizmoUtils.DrawPlane(position, rotation * targetRotation, Color.cyan, Vector2.one, 0.001f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(targetPosition, RADIUS);
            }

            void Bind(Vector3 position, Vector3 localNormal)
            {
                Vector3 normal = _target.TransformDirection(localNormal);
                if (GeomUtils.BindTargetToPlane(position, normal, ref targetPosition))
                {
                    bound = true;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(targetPosition, RADIUS);
                } 
            }
            
            if (_axes.HasFlag(Axes.X))
            {
                Bind(upperPosition, Vector3.left);
                Bind(lowerPosition, Vector3.right);
            }
            if (_axes.HasFlag(Axes.Y))
            {
                Bind(upperPosition, Vector3.down);
                Bind(lowerPosition, Vector3.up);
            }
            if (_axes.HasFlag(Axes.Z))
            {
                Bind(upperPosition, Vector3.back);
                Bind(lowerPosition, Vector3.forward);
            }

            if (bound)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPosition, RADIUS + 0.001f);
            }
            
            Gizmos.color = Color.white;
            
        }

    }
}