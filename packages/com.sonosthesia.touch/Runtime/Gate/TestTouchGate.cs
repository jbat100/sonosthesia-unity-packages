using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TestTouchGate : TouchGate
    {
        [SerializeField] private bool _block;

        protected override bool PerformCheck(TouchSource source, TouchActor actor) => !_block;
    }
}