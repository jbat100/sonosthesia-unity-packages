using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    public static class UIDemoWindowIntentKeys
    {
        public const string MAIN = "MAIN";
        public const string SCENE = "SCENE";
        public const string PERFORMANCE = "PERFORMANCE";
    }
    
    public class DemoUIWindowController : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intent;
        
        // Using this rather than MonoBehaviour on the object because it requires the controller game objects 
        // to be de activated so would require an intermediate game object to handle things (Awake setup etc)
        
        private class Fader : IDisposable
        {
            private CanvasGroup _canvasGroup;
            private CancellationTokenSource _cancellationTokenSource;

            public Fader(CanvasGroup canvasGroup)
            {
                _canvasGroup = canvasGroup;
            }

            public void Dispose()
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _canvasGroup = null;
            }

            public async UniTask<bool> FadeOutAndDeactivate(float duration)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                if (! await Fade(duration, false, _cancellationTokenSource.Token))
                {
                    return false;
                }
                _canvasGroup.gameObject.SetActive(false);
                return true;
            }

            public UniTask<bool> ActivateAndFadeIn(float duration)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                _canvasGroup.gameObject.SetActive(true);
                return Fade(duration, true, _cancellationTokenSource.Token);
            }

            private async UniTask<bool> Fade(float duration, bool fadeIn, CancellationToken cancellationToken)
            {
                if (!_canvasGroup)
                {
                    return false;
                }

                float alphaStart = _canvasGroup.alpha;
                float alphaEnd = fadeIn ? 1f : 0f;
                float startTime = Time.time;
                float endTime = startTime + duration;

                //Debug.LogWarning($"{this} {nameof(Fade)} running alpha <{alphaStart}-{alphaEnd} time <{startTime}-{endTime}>");
                while (Time.time < endTime)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        //Debug.LogWarning($"{this} {nameof(Fade)} return on cancellation");
                        // normal operation, don't throw...
                        return false;
                    }
                    float alpha = Mathf.Lerp(alphaStart, alphaEnd, Mathf.Clamp01((Time.time - startTime) / duration));
                    //Debug.Log($"{this} {nameof(Fade)} setting alpha {alpha}");
                    _canvasGroup.alpha = alpha;
                    
                    await UniTask.NextFrame();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    //Debug.LogWarning($"{this} {nameof(Fade)} return on cancellation");
                    // normal operation, don't throw...
                    return false;
                }
                
                //Debug.LogWarning($"{this} {nameof(Fade)} return on completed alpha transition {alphaEnd}");
                _canvasGroup.alpha = alphaEnd;
                return true;
            }
        }
        
        [SerializeField] private float _fadeDuration;
        
        [SerializeField] private CanvasGroup _mainUI;
        [SerializeField] private CanvasGroup _sceneUI;
        [SerializeField] private CanvasGroup _performanceUI;

        private readonly Dictionary<string, Fader> _faders = new();

        private IDisposable _intentSubscription;

        private void Cleanup()
        {
            _intentSubscription?.Dispose();
            _intentSubscription = null;
            foreach (Fader fader in _faders.Values)
            {
                fader.Dispose();
            }
            _faders.Clear();
        }
        
        protected virtual void OnEnable()
        {
            Cleanup();

            void SetupFader(string key, CanvasGroup canvasGroup)
            {
                _faders[key] = canvasGroup ? new Fader(canvasGroup) : null;
            }
            
            SetupFader(UIDemoWindowIntentKeys.MAIN, _mainUI);
            SetupFader(UIDemoWindowIntentKeys.SCENE, _sceneUI);
            SetupFader(UIDemoWindowIntentKeys.PERFORMANCE, _performanceUI);

            if (_intent)
            {
                _intentSubscription = _intent.Observable.Subscribe(HandleIntent);
            }
            
        }

        protected virtual void OnDisable()
        {
            Cleanup();
        }

        protected virtual void HandleIntent(Intent intent)
        {
            foreach (KeyValuePair<string, Fader> pair in _faders)
            {
                if (pair.Key == intent.Key)
                {
                    pair.Value.ActivateAndFadeIn(_fadeDuration).Forget();
                }
                else
                {
                    pair.Value.FadeOutAndDeactivate(_fadeDuration).Forget();
                }
            }
        }
    }
}


