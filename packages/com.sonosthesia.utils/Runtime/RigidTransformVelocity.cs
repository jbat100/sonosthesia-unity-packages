using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public class RigidTransformVelocity : MonoBehaviour
    {
        private RigidTransform? _previous;
        private RigidTransform? _velocity;

        public RigidTransform Velocity => _velocity ?? default(RigidTransform);

        protected virtual void Update()
        {
            RigidTransform current = transform.ToRigidTransform();
            
            if (_previous != null)
            {
                float delta = Time.deltaTime;
                float3 velPos = (current.pos - _previous.Value.pos) / delta;
                quaternion diff = math.mul(current.rot, math.inverse(_previous.Value.rot));
                Vector3 eulers = ((Quaternion) diff).eulerAngles / delta;
                Quaternion velRot = Quaternion.Euler(eulers);
                _velocity = new RigidTransform(velRot, velPos);
            }

            _previous = current;
        }

        protected virtual void OnEnable()
        {
            _previous = null;
            _velocity = null;
        }
    }
}