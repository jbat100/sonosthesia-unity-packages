using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public abstract class TouchEndpoint : InteractionEndpoint
    {
        [SerializeField] private bool _mute;
        public bool Mute => _mute;

        [SerializeField] private TouchNode _node;
        public TouchNode Node => _node;

        private float? _enableTime;
        public float? TimeSinceEnable => Time.time - _enableTime;
        
        protected virtual void OnEnable()
        {
            _enableTime = Time.time;
        }

        protected virtual void OnDisable()
        {
            _enableTime = null;
        }

        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            return !_node || _node.RequestPermission(other);
        }
    }
}