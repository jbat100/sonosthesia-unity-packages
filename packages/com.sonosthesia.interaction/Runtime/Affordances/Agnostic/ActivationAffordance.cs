using System.Collections.Generic;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ActivationAffordance<TEvent, TStreamContainer, TAffordance> : AgnosticAffordance<TEvent, TStreamContainer, TAffordance> 
        where TEvent : struct 
        where TStreamContainer : MonoBehaviour, IStreamContainer<TEvent>
        where TAffordance : ActivationAffordance<TEvent, TStreamContainer, TAffordance>
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