using System;
using UniRx;
using VContainer;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Application
{
    public class SceneSwitchFadeSignal : Signal<float>
    {
        private SceneSwitcher _switcher;
        
        [Inject]
        public void Construct(SceneSwitcher switcher)
        {
            _switcher = switcher;
        }
        
        private IDisposable _subscription;
        
        // TODO : investigate possible Tween library dependency
        
        private float _currentValue = 0f;
        private float _targetValue = 0f;
        private float _targetTime = 0f;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _switcher.StateObservable.Subscribe(state =>
            {
                switch (state)
                {
                    case SceneSwitcherState.FadeOut:
                        _targetValue = 1f;
                        _targetTime = Time.time + _switcher.Settings.FadeOut;
                        break;
                    case SceneSwitcherState.FadeIn:
                        _targetValue = 0f;
                        _targetTime = Time.time + _switcher.Settings.FadeIn;
                        break;
                }
            });
            
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
            
            Broadcast(_currentValue);
        }
    }
}