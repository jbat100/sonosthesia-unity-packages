using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Sonosthesia.XR
{
    // the palm transform does not have a representative rotation, this 

    public class XRPalmTracker : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] private XRHandSkeletonDriver _skeletonDriver;
        [SerializeField] private XRHandTrackingEvents _trackingEvents;

        [SerializeField] private Transform _palm;
        [SerializeField] private Transform _littleProximal;
        [SerializeField] private Transform _indexProximal;

        private IDisposable _subscription;
        
        protected void OnEnable()
        {
            if (!_target)
            {
                _target = transform;
            }
            
            if (!_skeletonDriver)
            {
                return;
            }

            _palm = GetSkeletonTransform(XRHandJointID.Palm);
            _littleProximal = GetSkeletonTransform(XRHandJointID.LittleProximal);
            _indexProximal = GetSkeletonTransform(XRHandJointID.IndexProximal);
        }

        protected void Update()
        {
            if (!_trackingEvents.handIsTracked)
            {
                return;
            }
            
            Vector3 palmPosition = _palm.position;
            Vector3 littleProximalPosition = _littleProximal.position;
            Vector3 indexProximalPosition = _indexProximal.position;
            
            _target.position = palmPosition;
            Vector3 littleProximalDirection = littleProximalPosition - palmPosition;
            Vector3 indexProximalDirection = indexProximalPosition - palmPosition;

            Vector3 palmUp = Vector3.Cross(littleProximalDirection, indexProximalDirection);
            _target.rotation = Quaternion.LookRotation(indexProximalDirection, palmUp);
        }

        private Transform GetSkeletonTransform(XRHandJointID jointID)
        {
            return _skeletonDriver.jointTransformReferences
                .First(reference => reference.xrHandJointID == jointID).jointTransform;
        }
    }
}