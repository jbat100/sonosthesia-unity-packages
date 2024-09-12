using System;
using Sonosthesia.Scaffold;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.XR
{
    // Note : spline handler requires too many assumptions to be made concerning the nature of the spline to be useful
    // in the case of the harp case, the plan seems to be something along the lines of an isosceles trapezoid
    // if this is the required shape constraint then better make it explicit in order to allow all resulting 
    // simplifications to be made rigorously. Making a package aimed at handling different types of specific quads 
    // https://en.wikipedia.org/wiki/Quadrilateral would be an interesting exercise
    
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

        [SerializeField] private AxisHandlerSettings _guideEndSettings;
    }
}