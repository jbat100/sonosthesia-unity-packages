using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class TestAffordanceGate : AffordanceGate
    {
        [SerializeField] private bool _block;

        protected override bool PerformCheck(IInteractionEvent e) => !_block;
    }
}