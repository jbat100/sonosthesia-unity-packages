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

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(targetPosition, RADIUS);
            bool bound = false;

            bound |= GeomUtils.LowerBind(_lower.position, targetRotation, _axes, ref targetPosition);
            bound |= GeomUtils.UpperBind(_upper.position, targetRotation, _axes, ref targetPosition);
            
            if (bound)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPosition, RADIUS + 0.001f);
            }
            
            Gizmos.color = Color.white;
            
        }

    }
}