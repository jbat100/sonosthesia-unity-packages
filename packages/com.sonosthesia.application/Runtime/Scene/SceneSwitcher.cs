using System;
using System.Threading;
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
        Unloading,
        Loading,
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
        [SerializeField] private bool _strict = true;
        [SerializeField] private float _fadeIn = 1f;
        [SerializeField] private float _fadeOut = 1f;
        
        private readonly BehaviorSubject<SceneSwitcherState> _stateSubject = new (SceneSwitcherState.Empty);
        public IObservable<SceneSwitcherState> StateObservable => _stateSubject.AsObservable();
        public SceneSwitcherState State
        {
            get => _stateSubject.Value;
            private set => _stateSubject.OnNext(value);
        }
        
        private readonly Subject<SceneSwitcherFade> _fadeSubject = new ();
        public IObservable<SceneSwitcherFade> FadeObservable => _fadeSubject.AsObservable();
        private UniTask Fade(bool fadeIn, float duration, CancellationToken cancellationToken)
        {
            _fadeSubject.OnNext(new SceneSwitcherFade(fadeIn, duration));
            return UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);
        }

        private readonly BehaviorSubject<string> _currentSubject = new(null);
        public IObservable<string> CurrentObservable => _currentSubject.AsObservable();
        public string Current
        {
            get => _currentSubject.Value;
            private set => _currentSubject.OnNext(value);
        }
        

        private IDisposable _intentSubscription;
        private readonly SemaphoreSlim _semaphore = new (1);
        private CancellationTokenSource _cancellationTokenSource = new ();

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
                    if (string.IsNullOrEmpty(i.Key))
                    {
                        return;
                    }
                    if (_strict && State != SceneSwitcherState.Idle)
                    {
                        Debug.LogWarning($"{this} dropped scene switch intent {i}");
                        return;
                    }
                    SwitchToScene(i.Key);
                });
            }
        }

        protected virtual void OnDisable() => _intentSubscription?.Dispose();
        
        public void SwitchToScene(string sceneName)
        {
            if (_strict && State != SceneSwitcherState.Idle)
            {
                throw new Exception($"{nameof(SwitchToScene)} called during invalid state {State}");
            }
            
            // Note : once the switch to R3 is made, we can use it as a synchronization mechanism
            
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            PerformSwitchToScene(sceneName, _cancellationTokenSource.Token).Forget();
        }

        private async UniTask PerformSwitchToScene(string sceneName, CancellationToken cancellationToken)
        {
            try
            {
                if (sceneName == Current)
                {
                    return;
                }
                
                Debug.Log($"{this} waiting for scene switch ");
                
                await _semaphore.WaitAsync(cancellationToken);
                
                cancellationToken.ThrowIfCancellationRequested();

                await Fade(true, _fadeIn, cancellationToken);

                if (!string.IsNullOrEmpty(Current))
                {
                    State = SceneSwitcherState.Unloading;
                    await SceneManager.UnloadSceneAsync(Current);
                }
                
                cancellationToken.ThrowIfCancellationRequested();

                if (!string.IsNullOrEmpty(sceneName))
                {
                    State = SceneSwitcherState.Loading;
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }

                Current = sceneName;
                
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                // notify error
            }
            finally
            {
                // we want to fade out no matter what
                Fade(false, _fadeOut, cancellationToken).Forget();
                State = SceneSwitcherState.Idle;
                _semaphore.Release();
            }
        }
    }
}