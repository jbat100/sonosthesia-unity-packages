using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    public class XRHandFollowHelper : MonoBehaviour
    {
        [Serializable]
        private class Element
        {
            [SerializeField] private Follower _follower;
            public Follower Follower => _follower;

            [SerializeField] private bool _active;
            public bool Active => _active;
        }
        
        [SerializeField] private XRHandTrackingEvents _trackingEvents;

        [SerializeField] private List<Element> _followers;

        private IDisposable _subscription;
        
        private void SetActiveFollower(bool active)
        {
            foreach (Element element in _followers.Where(e => e.Follower))
            {
                bool activeElement = active && element.Active;
                element.Follower.gameObject.SetActive(activeElement);
                if (activeElement)
                {
                    element.Follower.Align();
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

            _subscription = _trackingEvents.trackingChanged.AsObservable()
                .StartWith(_trackingEvents.handIsTracked)
                .Subscribe(SetActiveFollower);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
        }
    }
}