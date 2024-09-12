using System;
using Sonosthesia.Signal;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sonosthesia.Application
{
    // OnPostRender implementation inspired by
    // https://github.com/ValveSoftware/openvr/blob/master/samples/unity_keyboard_sample/Assets/SteamVR/Scripts/SteamVR_Fade.cs
    
    public class ScreenFade : MonoBehaviour
    {
        [SerializeField] private FloatSignalRelay _fade;
        [SerializeField] private Color _fadeColor = Color.black;
        
        private static Material _fadeMaterial = null;
        private IDisposable _subscription;
        private Color _currentColor = Color.clear;
        
        protected virtual void OnEnable()
        {
            if (_fadeMaterial == null)
            {
                _fadeMaterial = new Material(Shader.Find("UI/Unlit/Transparent"));
            }

            _subscription?.Dispose();
            if (_fade)
            {
                _subscription = _fade.Observable.Subscribe(f =>
                {
                    _currentColor = Color.Lerp(Color.clear, _fadeColor, f);
                });
            }
            else
            {
                _currentColor = Color.clear;
            }
            
            RenderPipelineManager.endCameraRendering += RenderPipelineManagerOnendCameraRendering;
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            
            RenderPipelineManager.endCameraRendering -= RenderPipelineManagerOnendCameraRendering;
        }
        
        private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            OnPostRender();
        }

        // OnPostRender is not called for SRPs 
        protected virtual void OnPostRender()
        {
            if (!(_currentColor.a > 0))
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