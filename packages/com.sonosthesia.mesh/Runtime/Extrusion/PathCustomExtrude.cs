using System;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sonosthesia.Mesh
{
    [ExecuteAlways]
    public class PathCustomExtrude : MonoBehaviour
    {
        [SerializeField] private ExtrusionPath _extrusionPath;
        
        
        private IDisposable _extrusionSubscription;
        private bool _dirty;
        
        private void Setup()
        {
            _dirty = true;
            _extrusionSubscription?.Dispose();
            if (_extrusionPath)
            {
                _extrusionSubscription = _extrusionPath.ChangeObservable.Subscribe(_ =>
                {
                    _dirty = true;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
#endif
                });
            }
        }

        protected virtual void OnValidate() => Setup();

        protected virtual void OnEnable() => Setup();

        protected virtual void OnDisable() => _extrusionSubscription?.Dispose();

        protected virtual void Update()
        {
            
        }
        
    }
}