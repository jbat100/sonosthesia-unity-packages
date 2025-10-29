using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TestInteractionGate : InteractionGate
    {
        [SerializeField] private bool _block;

        protected override bool PerformCheck(IInteractionEndpoint source, IInteractionEndpoint actor) => !_block;
    }
}