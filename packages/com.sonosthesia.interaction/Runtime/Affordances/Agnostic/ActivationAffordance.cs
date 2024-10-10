using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ActivationAffordance<TEvent, TAffordance> : AgnosticAffordance<TEvent, TAffordance> 
        where TEvent : struct, IInteractionEvent
        where TAffordance : ActivationAffordance<TEvent, TAffordance>
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