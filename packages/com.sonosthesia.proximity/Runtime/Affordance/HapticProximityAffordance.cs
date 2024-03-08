using System.Threading;
using Cysharp.Threading.Tasks;
using Sonosthesia.Haptic;
using UnityEngine;

namespace Sonosthesia.Proximity
{
    public class HapticProximityAffordance : ProximityAffordance
    {
        [SerializeField] private HapticController _controller;
        
        [SerializeField] private Vector3 _offset;
        
        [SerializeField] private AnimationCurve _distanceAmplitude;
        
        [SerializeField] private AnimationCurve _distanceDuration;
        
        [SerializeField] private AnimationCurve _distancePeriod;

        private CancellationTokenSource _rumbleCancellationTokenSource;

        protected virtual void Update()
        {
            Vector3 point = transform.TransformPoint(_offset);
            if (ClosestZone(point, out ProximityZone zone, out Vector3 target, out float distance))
            {
                Rumble(distance);
            }
            else
            {
                StopRumble();
            }
        }

        protected virtual void OnDisable()
        {
            StopRumble();
        }

        private void Rumble(float distance)
        {
            if (_rumbleCancellationTokenSource == null)
            {
                _rumbleCancellationTokenSource = new CancellationTokenSource();
                _controller.Rumble(() => _distanceAmplitude.Evaluate(distance),
                    () => _distanceDuration.Evaluate(distance),
                    () => _distancePeriod.Evaluate(distance),
                    _rumbleCancellationTokenSource.Token).Forget();
            }
        }

        private void StopRumble()
        {
            if (_rumbleCancellationTokenSource != null)
            {
                _rumbleCancellationTokenSource.Cancel();
                _rumbleCancellationTokenSource = null;   
            }
        }
    }
}