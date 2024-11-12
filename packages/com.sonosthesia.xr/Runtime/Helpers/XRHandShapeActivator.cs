using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace Sonosthesia.XR
{
    public class XRHandShapeActivator : MonoBehaviour
    {
        [Serializable]
        private class Element
        {
            [SerializeField] private GameObject _target;
            [SerializeField] private XRHandShape _shape;
            [SerializeField] private bool _active;

            public void Apply(XRHandJointsUpdatedEventArgs args) => SetActive(Check(args));

            public bool Check(XRHandJointsUpdatedEventArgs args) => _shape && _active && _shape.CheckConditions(args);

            public void SetActive(bool active)
            {
                if (_target)
                {
                    _target.SetActive(active);
                }
            }
        }
        
        [SerializeField] private XRHandProvider _hand;
        [SerializeField] private List<Element> _elements;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            DeactivateAll();
            if (_hand && _hand.TrackingEvents)
            {
                _subscription = _hand.TrackingEvents.jointsUpdated.AsObservable().Subscribe(args =>
                {
                    foreach (Element element in _elements)
                    {
                        element.Apply(args);
                    }
                });
            }
        }
        
        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            DeactivateAll();
        }

        private void DeactivateAll()
        {
            foreach (Element element in _elements)
            {
                element.SetActive(false);
            }
        }
    }
}