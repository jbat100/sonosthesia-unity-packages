using System;
using UniRx;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [ExecuteAlways]
    public abstract class PathCustomExtrude : MeshController
    {
        [SerializeField] private ExtrusionPath _path;

        [SerializeField] private int _segments = 10;
        
        [SerializeField] private float _scale = 1f;
        
        [SerializeField] private float _fade = 0f;

        [SerializeField] private ExtrusionVStrategy m_VStrategy = ExtrusionVStrategy.NormalizedRange;
        
        [SerializeField] private Vector2 _range = new Vector2(0f, 1f);
        
        [SerializeField] private bool _parallel;
        
        private IDisposable _pathSubscription;

        private NativeArray<RigidTransform> _pathPoints;
        
        private void Setup()
        {
            _pathSubscription?.Dispose();
            if (_path)
            {
                _pathSubscription = _path.ChangeObservable.Subscribe(_ => RequestRebuild());
            }
        }

        protected override void OnValidate()
        {
            Setup();
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            Setup();
            base.OnEnable();
        }

        protected virtual void OnDisable()
        {
            _pathSubscription?.Dispose();
            _pathPoints.Dispose();
        }

        protected sealed override void PopulateMeshData(UnityEngine.Mesh.MeshData data)
        {
            ExtrusionSettings extrusionSettings = new ExtrusionSettings(_path.GetLength(), _segments, _path.Closed, _range, _scale, _fade, m_VStrategy);
            _pathPoints.TryReusePersistentArray(_segments);
            _path.Populate(ref _pathPoints, _range, _segments);
            PopulateMeshData(data, _pathPoints, extrusionSettings, _parallel);
        }

        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data, NativeArray<RigidTransform> pathPoints, ExtrusionSettings extrusionSettings, bool parallel);
    }
}