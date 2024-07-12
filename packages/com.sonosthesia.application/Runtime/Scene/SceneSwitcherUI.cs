using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia 
{
    public class SceneSwitcherUI : MonoBehaviour
    {
        [SerializeField] private Text _stateText;

        [Serializable]
        public class Choice
        {
            [SerializeField] private Button _button;
            public Button Button => _button; 
                
            [SerializeField] private string _name;
            public string Name => _name;
        }
        
        [SerializeField] private Choice[] _choices;

        private string _current;
        private readonly SemaphoreSlim _semaphore = new (1);
        private CancellationTokenSource _cancellationTokenSource = new ();

        protected virtual void Start()
        {
            foreach (Choice choice in _choices)
            {
                choice.Button.onClick.AddListener(() => SwitchToScene(choice.Name));
            }            
        }

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


