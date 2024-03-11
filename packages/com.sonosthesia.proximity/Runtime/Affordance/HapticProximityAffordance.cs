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

        private float _lastDistance;

        protected virtual void Update()
        {
            Vector3 point = transform.TransformPoint(_offset);
            if (ClosestZone(point, out ProximityZone zone, out Vector3 target, out float distance))
            {
                _lastDistance = distance;
                Rumble(true);
            }
            else
            {
                Rumble(false);
            }
        }

        protected virtual void OnDisable()
        {
            Rumble(false);
        }

        private void Rumble(bool on)
        {
            switch (on)
            {
                case true when _rumbleCancellationTokenSource == null:
                    _rumbleCancellationTokenSource = new CancellationTokenSource();
                    _controller.Rumble(() => _distanceAmplitude.Evaluate(_lastDistance),
                        () => _distanceDuration.Evaluate(_lastDistance),
                        () => _distancePeriod.Evaluate(_lastDistance),
                        _rumbleCancellationTokenSource.Token).Forget();
                    break;
                case false when _rumbleCancellationTokenSource != null:
                    _rumbleCancellationTokenSource.Cancel();
                    _rumbleCancellationTokenSource = null;
                    break;
            }
        }
    }
}