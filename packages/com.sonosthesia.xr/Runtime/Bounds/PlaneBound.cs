using UnityEngine;

namespace Sonosthesia.XR
{
    // this turned out useless because it's the interactables which define the direction, not the bounds
    
    public class PlaneBound : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;

        public Plane Plane => new Plane(transform.position, transform.TransformDirection(_direction));

    }
}