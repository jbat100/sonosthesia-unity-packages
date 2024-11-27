using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TestTouchActorGate : TouchActorGate
    {
        [SerializeField] private bool _block;

        protected override bool PerformCheck(TouchSource source, TouchActor actor) => !_block;
    }
}