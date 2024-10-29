using UnityEngine;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFade : AbstractFade
    {
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void Apply(float fade)
        {
            _canvasGroup.alpha = 1f - fade;
        }
    }
}