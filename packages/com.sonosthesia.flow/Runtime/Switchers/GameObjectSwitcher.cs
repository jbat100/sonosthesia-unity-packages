using UnityEngine;

namespace Sonosthesia.Flow
{
    public class GameObjectSwitcher : Switcher<GameObject>
    {
        protected override void Switch(GameObject target, bool on)
        {
            if (target.activeSelf != on)
            {
                target.SetActive(on);    
            }
        }
    }
}