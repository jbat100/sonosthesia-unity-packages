using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class StartTouchAffordanceGate : TypedAffordanceGate<TouchEvent>
    {
        [SerializeField] private TouchStart _start;

        protected override bool PerformCheck(TouchEvent e) => e.touchData.Start == _start;
    }
}