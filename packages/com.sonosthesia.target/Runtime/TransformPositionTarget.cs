using UnityEngine;

namespace Sonosthesia.Target
{
    public class TransformPositionTarget : BlendTarget<Vector3, Vector3Blender>
    {
        [SerializeField] private bool _local;
        
        protected override Vector3 Reference => _local ? transform.localPosition : transform.position;

        protected override void ApplyBlended(Vector3 value)
        {
            if (_local)
            {
                transform.localPosition = value;
            }
            else
            {
                transform.position = value;
            }
        }
    }
}