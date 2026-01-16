using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class InteractionGate : MonoBehaviour
    {
        [SerializeField] private bool _bypass;

        public bool Check(IInteractionEndpoint source, IInteractionEndpoint actor)
        {
            return _bypass || PerformCheck(source, actor);
        }
        
        protected abstract bool PerformCheck(IInteractionEndpoint source, IInteractionEndpoint actor);
    }
}