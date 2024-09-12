using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Application
{
    public class CompositeAudioController : BaseAudioController
    {
        [SerializeField] private List<BaseAudioController> _controllers;
        
        public override void Play(string eventName)
        {
            foreach (BaseAudioController controller in _controllers)
            {
                controller.Play(eventName);
            }
        }
    }
}