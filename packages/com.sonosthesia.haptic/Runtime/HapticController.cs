using System;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Cysharp.Threading.Tasks;

namespace Sonosthesia.Haptic
{
    public class HapticController : MonoBehaviour
    {
        [SerializeField] private XRBaseController _controller;

        public bool SendHapticImpulse(float amplitude, float duration)
        {
            return _controller && _controller.SendHapticImpulse(amplitude, duration);
        }

        public async UniTask Rumble(Func<float> amplitudeProvider, Func<float> durationProvider, Func<float> periodProvider, CancellationToken cancellationToken)
        {
            // may seem long winded but allows changes in period to be taken into account immediately 
            
            while (!cancellationToken.IsCancellationRequested)
            {
                float iterationStartTime = Time.time;

                float duration = durationProvider();
                float amplitude = amplitudeProvider();
                SendHapticImpulse(amplitude, duration);
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.NextFrame(cancellationToken);
                    
                    float period = periodProvider();
                    float iterationEndTime = iterationStartTime + period;

                    if (Time.time > iterationEndTime)
                    {
                        break;
                    }
                }
            }
            
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}