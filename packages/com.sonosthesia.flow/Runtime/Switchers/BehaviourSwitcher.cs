using UnityEngine;

namespace Sonosthesia.Flow
{
    public class BehaviourSwitcher : Switcher<Behaviour>
    {
        protected override void Switch(Behaviour target, bool on)
        {
            if (target.enabled != on)
            {
                target.enabled = on;
            }
        }
    }
}