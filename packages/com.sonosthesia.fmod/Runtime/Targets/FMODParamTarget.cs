using System;
using UnityEngine;
using UniRx;
using FMOD;
using FMOD.Studio;
using Sonosthesia.Signal;
using Unity.Mathematics;

namespace Sonosthesia.FMOD
{
    public class FMODParamTarget : Target<float>
    {
        public enum RangeHandling
        {
            Clamp,
            Mute,
            Force
        }
        
        [SerializeField] private FMODInstance _instance;
        
        [SerializeField] private string _parameterName;

        [SerializeField] private RangeHandling _rangeHandling = RangeHandling.Clamp;
        
        private IDisposable _subscription;
        private EventInstance _eventInstance;
        private EventDescription _eventDescription;
        private PARAMETER_DESCRIPTION _parameterDescription;
        private bool _ready;
        
        // https://qa.fmod.com/t/getting-parameter-ids-is-not-straightforward/19776

        protected override void OnEnable()
        {
            base.OnEnable();
            _subscription?.Dispose();
            _subscription = _instance.InstanceObservable.Subscribe(i =>
            {
                _eventInstance = i;
                RefreshParameterDescription();
                
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _subscription?.Dispose();
            Clear();
        }

        protected virtual void OnValidate()
        {
            RefreshParameterDescription();
        }

        private void Clear()
        {
            _ready = false;
            _eventInstance = default;
            _eventDescription = default;
            _parameterDescription = default;
        }

        private void RefreshParameterDescription()
        {
            _ready = false;
            if (_eventInstance.isValid())
            {
                RESULT result = _eventInstance.getDescription(out _eventDescription);
                if (result != RESULT.OK)
                {
                    UnityEngine.Debug.LogWarning($"{this} failed getDescription {result}");
                    return;
                }
                result = _eventDescription.getParameterDescriptionByName(_parameterName, out _parameterDescription);
                if (result != RESULT.OK)
                {
                    UnityEngine.Debug.LogWarning($"{this} failed getParameterDescriptionByName {_parameterName} {result}");
                    return;
                }

                UnityEngine.Debug.LogWarning($"{this} {nameof(RefreshParameterDescription)} succeeded");
                _ready = true;
            }
            else
            {
                Clear();
            }
        }

        protected override void Apply(float value)
        {
            if (_ready)
            {
                if (value > _parameterDescription.maximum || value < _parameterDescription.minimum)
                {
                    switch (_rangeHandling)
                    {
                        case RangeHandling.Clamp:
                            value = math.clamp(value, _parameterDescription.minimum, _parameterDescription.maximum);
                            // UnityEngine.Debug.LogWarning($"{this} clamped out of range value {value}");
                            break;
                        case RangeHandling.Mute:
                            // UnityEngine.Debug.LogWarning($"{this} muted out of range value {value}");
                            return;
                        case RangeHandling.Force:
                            // UnityEngine.Debug.LogWarning($"{this} forcing out of range value {value}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                
                RESULT result = _eventInstance.setParameterByID(_parameterDescription.id, value);
                if (result != RESULT.OK)
                {
                    UnityEngine.Debug.LogWarning($"{this} failed setParameterByID {result}");
                    Clear();
                }
            }
        }
    }
}