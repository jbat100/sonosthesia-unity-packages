using System;
using Cysharp.Threading.Tasks;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Sonosthesia.Application
{
    public class SceneSwitcher : ISceneSwitcher, IDisposable
    {
        private readonly SceneSwitcherSettings _settings;
        public SceneSwitcherSettings Settings => _settings;

        private LifetimeScope _parentScope;
        
        public SceneSwitcher(SceneSwitcherSettings settings, LifetimeScope parentScope)
        {
            _settings = settings;
            _parentScope = parentScope;
        }
        
        private BehaviorSubject<SceneSwitcherState> _stateSubject = new (SceneSwitcherState.Idle);
        public IObservable<SceneSwitcherState> StateObservable => _stateSubject.AsObservable();
        public SceneSwitcherState State
        {
            get => _stateSubject.Value;
            private set
            {
                Debug.Log($"{this} state {value}");
                _stateSubject.OnNext(value);
            }
        }

        private BehaviorSubject<string> _currentSubject = new(null);
        public IObservable<string> CurrentObservable => _currentSubject.AsObservable();
        public string Current
        {
            get => _currentSubject.Value;
            private set => _currentSubject.OnNext(value);
        }

        private IDisposable _intentSubscription;

        public async UniTask SwitchToScene(string sceneName)
        {
            if (sceneName == Current)
            {
                return;
            }

            try
            {
                State = SceneSwitcherState.FadeOut;

                await UniTask.Delay(TimeSpan.FromSeconds(Settings.FadeOut));

                if (!string.IsNullOrEmpty(Current))
                {
                    State = SceneSwitcherState.Unloading;
                    await SceneManager.UnloadSceneAsync(Current);
                }
            }
            catch (Exception e)
            {
                // TODO: have observable for error UI
                Debug.LogException(e);
            }
            
            try
            {
                if (!string.IsNullOrEmpty(sceneName))
                {
                    State = SceneSwitcherState.Loading;
                    using (LifetimeScope.EnqueueParent(_parentScope))
                    {
                        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }
                }

                Current = sceneName;
            }
            catch (Exception e)
            {
                // TODO: have observable for error UI
                Debug.LogException(e);
            }
            
            State = SceneSwitcherState.FadeIn;
            
            await UniTask.Delay(TimeSpan.FromSeconds(Settings.FadeIn));
            
            State = SceneSwitcherState.Idle;
        }

        public void Dispose()
        {
            Debug.LogWarning($"{this} {nameof(Dispose)}");
            RxUtils.Cleanup(ref _stateSubject);
            RxUtils.Cleanup(ref _currentSubject);
        }
    }
}