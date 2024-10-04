using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ActivationAffordance<TEvent, TContainer, TAffordance> : AgnosticAffordance<TEvent, TContainer, TAffordance> 
        where TEvent : struct 
        where TContainer : MonoBehaviour, IEventStreamContainer<TEvent>
        where TAffordance : ActivationAffordance<TEvent, TContainer, TAffordance>
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