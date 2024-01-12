using UnityEngine;

namespace Sonosthesia.Touch
{
    public class BaseTriggerActor : TriggerStream
    {
        // can be used to filter actors or to allow one source to have different responses 
        [SerializeField] private int _domain;

        [SerializeField] private TriggerNode _node;
        public TriggerNode Node => _node;

        public virtual bool RequestPermission(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            if (_node)
            {
                return _node.RequestPermission(other);
            }

            return true;
        }
    }
}