using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Sonosthesia.Application
{
    public class SceneSwitcher : ISceneSwitcher, IDisposable
    {
        public SceneSwitcherSettings Settings { get; }

        private readonly LifetimeScope _parentScope;
        private readonly ApplicationState _applicationState;

        public SceneSwitcher(SceneSwitcherSettings settings, ApplicationState applicationState, LifetimeScope parentScope)
        {
            Settings = settings;
            _parentScope = parentScope;
            _applicationState = applicationState;
        }

        private readonly ReactiveProperty<SceneSwitcherState> _state = new(SceneSwitcherState.Idle);
        public IReadOnlyReactiveProperty<SceneSwitcherState> State => _state;

        private readonly ReactiveProperty<string> _current = new (null);
        public IReadOnlyReactiveProperty<string> Current => _current;
        
        public async UniTask SwitchToScene(string sceneName)
        {
            if (sceneName == Current.Value)
            {
                return;
            }

            try
            {
                _state.Value = SceneSwitcherState.FadeOut;

                await UniTask.Delay(TimeSpan.FromSeconds(Settings.FadeOut));
                

                if (!string.IsNullOrEmpty(Current.Value))
                {
                    _state.Value = SceneSwitcherState.Unloading;
                    await SceneManager.UnloadSceneAsync(Current.Value);
                }
            }
            catch (Exception e)
            {
                // TODO: have observable for error UI
                Debug.LogException(e);
            }

            _applicationState.activeUI.Value = false;
            
            try
            {
                if (!string.IsNullOrEmpty(sceneName))
                {
                    _state.Value = SceneSwitcherState.Loading;
                    using (LifetimeScope.EnqueueParent(_parentScope))
                    {
                        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    }
                }

                _current.Value = sceneName;
            }
            catch (Exception e)
            {
                // TODO: have observable for error UI
                Debug.LogException(e);
            }
            
            _state.Value = SceneSwitcherState.FadeIn;
            
            await UniTask.Delay(TimeSpan.FromSeconds(Settings.FadeIn));
            
            _state.Value = SceneSwitcherState.Idle;
        }

        public void Dispose()
        {
            _state.Dispose();
            _current.Dispose();
        }
    }
}