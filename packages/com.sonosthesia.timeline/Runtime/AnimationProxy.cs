using System;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline
{
    public class AnimationProxy : MonoBehaviour
    {
        [Serializable]
        public struct Proxy
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