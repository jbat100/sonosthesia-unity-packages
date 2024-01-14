using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ActivationPointerAffordance : PointerAgnosticAffordance
    {
        [SerializeField] private List<GameObject> _targets;
        
        protected override void OnEventCountChanged(int count)
        {
            bool active = count > 0;
            foreach (GameObject target in _targets)
            {
                target.SetActive(active);
            }    
        }
    }
}