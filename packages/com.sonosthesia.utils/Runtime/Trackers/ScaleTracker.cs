using UnityEngine;

namespace Sonosthesia.Utils
{
    [ExecuteInEditMode]
    public class ScaleTracker : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] private Vector3 _factor = Vector3.one;

        [SerializeField] private Vector3 _offset;
        
        protected virtual void LateUpdate()
        {
            if (_target)
            {
                transform.localScale = Vector3.Scale(_target.localScale, _factor) + _offset;
            }
        }
    }
}