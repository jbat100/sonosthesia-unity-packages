using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ActivationAffordance<TEvent, TSource> : AgnosticAffordance<TEvent, TSource> 
        where TEvent : struct where TSource : MonoBehaviour, IEventStreamContainer<TEvent>
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