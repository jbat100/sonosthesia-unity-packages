using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    public class XRHandFollowHelper : MonoBehaviour
    {
        [SerializeField] private XRHandTrackingEvents _trackingEvents;

        [SerializeField] private List<Follower> _followers;

        private IDisposable _subscription;
        
        private void SetActiveFollower(bool active)
        {
            foreach (Follower follower in _followers)
            {
                follower.gameObject.SetActive(active);
                if (active)
                {
                    follower.Align();
                }
            }    
        }
        
        protected void OnEnable()
        {
            _subscription?.Dispose();

            if (!_trackingEvents)
            {
                SetActiveFollower(false);
                return;
            }

            _subscription = _trackingEvents.trackingChanged.AsObservable().Subscribe(SetActiveFollower);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}