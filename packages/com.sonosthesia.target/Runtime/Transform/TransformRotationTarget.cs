using UnityEngine;

namespace Sonosthesia.Target
{
    public class TransformRotationTarget : BlendTarget<Quaternion, QuaternionBlender>
    {
        [SerializeField] private bool _local = true;

        protected override Quaternion Reference => _local ? transform.localRotation : transform.rotation;
        protected override void ApplyBlended(Quaternion value)
        {
            if (_local)
            {
                transform.localRotation = value;  
            }
            else
            {
                transform.rotation = value;     
            }
        }

    }
}