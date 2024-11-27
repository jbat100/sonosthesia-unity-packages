using UnityEngine;

namespace Sonosthesia.Application
{
    public class RendererSceneFade : AbstractFade
    {
        [SerializeField] private Renderer _renderer;

        protected override void Apply(float fade)
        {
            if (Mathf.Approximately(fade, 0))
            {
                _renderer.enabled = false;
                return;
            }
            
            _renderer.enabled = true;
            _renderer.material.color = new Color(0, 0, 0, fade);
        }
    }
}