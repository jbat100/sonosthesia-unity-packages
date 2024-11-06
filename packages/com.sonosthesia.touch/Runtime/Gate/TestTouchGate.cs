using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TestTouchGate : TouchGate
    {
        [SerializeField] private bool _block;

        public override bool Check(TouchSource source, TouchActor actor) => !_block;
    }
}