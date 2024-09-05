using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sonosthesia.Application
{
    // Note : using this script on a disabled GameObject should still work but we can't do an auto GetComponent 
    // to setup the CanvasGroup reference in Awake as it is not called if the GameObject is inactive on instantiation.
    // So the _canvasGroup reference is required to be set explicitly for serialization.
    
    public class CanvasGroupController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool? _active;
        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void OnEnable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        protected virtual void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        public UniTask Toggle(float duration)
        {
            if (_active == true)
            {
                return FadeOutAndDeactivate(duration);
            }
            else
            {
                return ActivateAndFadeIn(duration);
            }
        }

        public async UniTask<bool> FadeOutAndDeactivate(float duration)
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _active = false;
            if (! await Fade(duration, false, _cancellationTokenSource.Token))
            {
                return false;
            }
            _canvasGroup.gameObject.SetActive(false);
            return true;
        }

        public UniTask<bool> ActivateAndFadeIn(float duration)
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _active = true;
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

            while (Time.time < endTime)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // normal operation, don't throw...
                    return false;
                }
                _canvasGroup.alpha = Mathf.Lerp(alphaStart, alphaEnd, Mathf.Clamp01((Time.time - startTime) / duration));
                await UniTask.NextFrame();
            }

            if (cancellationToken.IsCancellationRequested)
            {
                // normal operation, don't throw...
                return false;
            }
            _canvasGroup.alpha = alphaEnd;
            return true;
        }
    }
}