using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Application
{
    // OnPostRender implementation inspired by
    // https://github.com/ValveSoftware/openvr/blob/master/samples/unity_keyboard_sample/Assets/SteamVR/Scripts/SteamVR_Fade.cs

    public class ScreenFade : AbstractFade
    {
        private const float THRESHOLD = 1e-4f;
        
        [SerializeField] private Color _fadeColor = Color.black;
        
        private static Material _fadeMaterial = null;
        private Color _currentColor = Color.clear;

        protected override void Apply(float fade)
        {
            _currentColor = Color.Lerp(Color.clear, _fadeColor, fade);
        }

        protected virtual void OnEnable()
        {
            if (_fadeMaterial == null)
            {
                _fadeMaterial = new Material(Shader.Find("UI/Unlit/Transparent"));
            }
            RenderPipelineManager.endCameraRendering += RenderPipelineManagerOnendCameraRendering;
        }

        protected virtual void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManagerOnendCameraRendering;
        }
        
        private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            OnPostRender();
        }

        // OnPostRender is not called for SRPs 
        protected virtual void OnPostRender()
        {
            if (_currentColor.a < THRESHOLD)
            {
                return;
            }
            
            GL.PushMatrix();
            GL.LoadOrtho();
            _fadeMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Color(_currentColor);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.End();
            GL.PopMatrix();
        }
    }
}