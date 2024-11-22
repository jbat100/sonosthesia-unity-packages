using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sonosthesia.XR
{
    // Used as a substitute for XRHandShapeActivator, more for debug purposes that anything else
    
    public class XRControllerActivator : MonoBehaviour
    {
        [Serializable]
        public class Threshold
        {
            public enum ThresholdType
            {
                None,
                Equal,
                Below,
                Above
            }

            [SerializeField] private ThresholdType _threshold;
            
            [SerializeField] private float _value;

            public bool Check(float value)
            {
                return _threshold switch
                {
                    ThresholdType.Below => value <= _value,
                    ThresholdType.Above => value >= _value,
                    ThresholdType.Equal => Mathf.Approximately(value, _value),
                    _ => true
                };
            }
        }
        
        [Serializable]
        public class Element
        {
            [SerializeField] private bool _active;
            [SerializeField] private GameObject _target;
            [SerializeField] private Threshold _grip;
            [SerializeField] private Threshold _trigger;

            public void Apply(float grip, float trigger) => SetActive(_active && Check(grip, trigger));
            
            public bool Check(float grip, float trigger) => _grip.Check(grip) && _trigger.Check(trigger);

            public void SetActive(bool active)
            {
                if (_target)
                {
                    _target.SetActive(active);
                }
            }
        }
        
        [SerializeField] private InputActionProperty _grip = new (new InputAction("Grip", expectedControlType: "Axis"));
        
        [SerializeField] private InputActionProperty _trigger = new (new InputAction("Trigger", expectedControlType: "Axis"));

        [SerializeField] private List<Element> _targets;

        protected virtual void Update()
        {
            float grip = _grip.action.ReadValue<float>();
            float trigger = _trigger.action.ReadValue<float>();

            foreach (Element target in _targets)
            {
                target.Apply(grip, trigger);
            }
        }

    }
}