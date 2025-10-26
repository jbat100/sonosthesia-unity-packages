using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class ActivationChannelCountAffordance : ChannelCountAffordance
    {
        [SerializeField] private List<GameObject> _targets;
        
        protected override void OnCountUpdated(int count)
        {
            bool active = count > 0;
            foreach (GameObject target in _targets)
            {
                if (target)
                {
                    target.SetActive(active);
                }
            }
        }
    }
}