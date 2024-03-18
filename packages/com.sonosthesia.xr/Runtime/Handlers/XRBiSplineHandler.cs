using System;
using Sonosthesia.Scaffold;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.XR
{
    public class XRBiSplineHandler : MonoBehaviour
    {
        [SerializeField] private BiSplineConfiguration _configuration;
        
        [Serializable]
        public class PoseHandlerSettings
        {
            [SerializeField] private XRGrabInteractable _interactablePrefab;

            [SerializeField] private Vector3 _offset;
        }
        
        [Serializable]
        public class AxisHandlerSettings
        {
            [SerializeField] private XRGrabInteractable _interactablePrefab;

            [SerializeField] private Vector3 _offset;

            [SerializeField] private Vector3 _direction;

            [SerializeField] private float _lowerBound;

            [SerializeField] private float _upperBound;
        }

        [SerializeField] private PoseHandlerSettings _mainHandlerSettings;

        [SerializeField] private AxisHandlerSettings _orientationStartSettings;

        [SerializeField] private AxisHandlerSettings _orientationEndSettings;

        [SerializeField] private AxisHandlerSettings _guidEndSettings;

        protected virtual void OnEnable()
        {
            
        }
        
    }
}