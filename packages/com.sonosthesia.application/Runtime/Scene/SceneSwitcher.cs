using System;
using Cysharp.Threading.Tasks;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonosthesia.Application
{

    public enum SceneSwitcherState
    {
        Empty,
        FadeOut,
        Unloading,
        Loading,
        FadeIn,
        Idle
    }

    public readonly struct SceneSwitcherFade
    {
        public readonly bool In;
        public readonly float Duration;

        public SceneSwitcherFade(bool fadeIn, float duration = 0f)
        {
            In = fadeIn;
            Duration = duration;
        }
    }
    
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intents;
        
        [SerializeField] private float _fadeIn = 1f;
        public float FadeIn => _fadeIn;
        
        [SerializeField] private float _fadeOut = 1f;
        public float FadeOut => _fadeOut;
        
        private readonly BehaviorSubject<SceneSwitcherState> _stateSubject = new (SceneSwitcherState.Empty);
        public IObservable<SceneSwitcherState> StateObservable => _stateSubject.AsObservable();
        public SceneSwitcherState State
        {
            get => _stateSubject.Value;
            private set => _stateSubject.OnNext(value);
        }
        

        private readonly BehaviorSubject<string> _currentSubject = new(null);
        public IObservable<string> CurrentObservable => _currentSubject.AsObservable();
        public string Current
        {
            get => _currentSubject.Value;
            private set => _currentSubject.OnNext(value);
        }
        
        private IDisposable _intentSubscription;

        protected virtual void Start()
        {
            State = SceneSwitcherState.Idle;
        }

        protected virtual void OnEnable()
        {
            _intentSubscription?.Dispose();
            if (_intents)
            {
                _intentSubscription = _intents.Observable.Subscribe(i =>
                {
                    if (State != SceneSwitcherState.Idle)
                    {
                        Debug.LogWarning($"{this} dropped scene switch intent {i}");
                        return;
                    }
                    SwitchToScene(i.Key).Forget();
                });
            }
        }

        protected virtual void OnDisable() => _intentSubscription?.Dispose();

        public async UniTask SwitchToScene(string sceneName)
        {
            if (sceneName == Current)
            {
                return;
            }

            try
            {
                State = SceneSwitcherState.FadeOut;
            
                await UniTask.Delay(TimeSpan.FromSeconds(_fadeOut));

                if (!string.IsNullOrEmpty(Current))
                {
                    State = SceneSwitcherState.Unloading;
                    await SceneManager.UnloadSceneAsync(Current);
                }
            
                if (!string.IsNullOrEmpty(sceneName))
                {
                    State = SceneSwitcherState.Loading;
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }

                Current = sceneName;
            }
            catch (Exception e)
            {
                // notify error
            }
            
            State = SceneSwitcherState.FadeIn;
            
            await UniTask.Delay(TimeSpan.FromSeconds(_fadeIn));
            
            State = SceneSwitcherState.Idle;
        }
    }
}