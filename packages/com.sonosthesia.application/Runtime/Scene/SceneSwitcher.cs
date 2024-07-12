using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    
    public class SceneSwitcher : MonoBehaviour
    {
        private BehaviorSubject<SceneSwitcherState> _stateSubject;
        public IObservable<SceneSwitcherState> StateObservable => _stateSubject.AsObservable();

        private string _current;
        private readonly SemaphoreSlim _semaphore = new (1);
        private CancellationTokenSource _cancellationTokenSource = new ();
        
        public void SwitchToScene(string sceneName)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            PerformSwitchToScene(sceneName, _cancellationTokenSource.Token).Forget();
        }

        private async UniTask PerformSwitchToScene(string sceneName, CancellationToken cancellationToken)
        {
            try
            {
                if (sceneName == _current)
                {
                    return;
                }
                
                await _semaphore.WaitAsync(cancellationToken);
                
                cancellationToken.ThrowIfCancellationRequested();

                // visual animation

                if (!string.IsNullOrEmpty(_current))
                {
                    await SceneManager.UnloadSceneAsync(_current);
                }
                
                cancellationToken.ThrowIfCancellationRequested();

                if (!string.IsNullOrEmpty(sceneName))
                {
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                }

                // visual animation

                _current = sceneName;
                
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                // notify error
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}