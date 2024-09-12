using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(SceneSwitcher))]
    public class SceneSwitchFader : MonoBehaviour
    {
        [SerializeField] private FloatSignalRelay _fade;
        
        private SceneSwitcher _switcher;
        private IDisposable _subscription;
        
        // TODO : investigate possible Tween library dependency
        
        private float _currentValue = 0f;
        private float _targetValue = 0f;
        private float _targetTime = 0f;
        
        protected virtual void Awake()
        {
            _switcher = GetComponent<SceneSwitcher>();
        }

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_switcher)
            {
                _subscription = _switcher.StateObservable.Subscribe(state =>
                {
                    switch (state)
                    {
                        case SceneSwitcherState.FadeOut:
                            _targetValue = 1f;
                            _targetTime = Time.time + _switcher.FadeOut;
                            break;
                        case SceneSwitcherState.FadeIn:
                            _targetValue = 0f;
                            _targetTime = Time.time + _switcher.FadeIn;
                            break;
                    }
                });
            }
        }

        protected virtual void OnDisable() => _subscription?.Dispose();

        protected virtual void Update()
        {
            if (Time.time >= _targetTime)
            {
                _currentValue = _targetValue;
            }
            else
            {
                float distance = _targetValue - _currentValue;
                float duration = _targetTime - Time.time;
                _currentValue += (distance / duration) * Time.deltaTime;
            }
            
            _fade.Broadcast(_currentValue);
        }
    }
}