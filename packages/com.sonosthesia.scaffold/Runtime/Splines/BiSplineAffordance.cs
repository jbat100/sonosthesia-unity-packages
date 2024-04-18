using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BiSplineAffordance), true)]
    public class BiSplineAffordanceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BiSplineAffordance affordance = (BiSplineAffordance)target;
            if(GUILayout.Button("Refresh"))
            {
                affordance.ForceRefreshAffordance();
            }
        }
    }
#endif
    
    public abstract class BiSplineAffordance : MonoBehaviour
    {
        [SerializeField] private BiSplineConfiguration _configuration;
        protected BiSplineConfiguration Configuration => _configuration;
        
        private IDisposable _configurationSubscription;

        protected virtual void Setup()
        {
            _configurationSubscription?.Dispose();
            if (_configuration)
            {
                _configurationSubscription = _configuration.ChangeObservable.StartWith(Unit.Default).Subscribe(_ => RefreshAffordance());   
            }
        }

        protected virtual void OnEnable() => Setup();

        protected virtual void OnDisable() => _configurationSubscription?.Dispose();

        protected virtual void OnValidate() => Setup();

        protected abstract void RefreshAffordance();

        public void ForceRefreshAffordance() => RefreshAffordance();
    }
}