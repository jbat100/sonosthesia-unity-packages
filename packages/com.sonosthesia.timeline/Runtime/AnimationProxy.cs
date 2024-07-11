using System;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline
{
    public class AnimationProxy : MonoBehaviour
    {
        // Used to mark proxy containing objects for automatic signal creation, nested IProxyContainers are supported
        // and will result in nested hierarchy of GameObjects with FloatSignal components 
        public interface IProxyContainer
        {
            
        }
        
        // Note Proxy is a class not a struct, this is so that it can be modified by editor scripts to like the 
        // signal field to dynamically created FloatSignal components
        
        [Serializable]
        public class Proxy
        {
            public float value;
            public Signal<float> signal;

            public void Update()
            {
                if (signal)
                {
                    signal.Broadcast(value);
                }
            }
        }
    }
}