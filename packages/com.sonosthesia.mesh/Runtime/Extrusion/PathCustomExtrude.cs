﻿using System;
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

        [SerializeField] private PathProcessor _processor;
        
        [SerializeField] private int _segments = 10;
        
        [SerializeField] private float _scale = 1f;
        
        [SerializeField] private float _fade = 0f;

        [SerializeField] private ExtrusionVStrategy m_VStrategy = ExtrusionVStrategy.NormalizedRange;
        
        [SerializeField] private Vector2 _range = new Vector2(0f, 1f);
        
        [SerializeField] private bool _parallel;
        
        private CompositeDisposable _rebuildSubscriptions = new ();

        private NativeArray<RigidTransform> _pathPoints;
        
        private void Setup()
        {
            _rebuildSubscriptions.Clear();
            if (_path)
            {
                _rebuildSubscriptions.Add(_path.ChangeObservable.Subscribe(_ => RequestRebuild()));
            }
            if (_processor)
            {
                _rebuildSubscriptions.Add(_processor.ChangeObservable.Subscribe(_ => RequestRebuild()));
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
            _rebuildSubscriptions.Clear();
            _pathPoints.Dispose();
        }

        protected sealed override void PopulateMeshData(UnityEngine.Mesh.MeshData data)
        {
            if (!_path)
            {
                return;
            }
            
            ExtrusionSettings extrusionSettings = new ExtrusionSettings(_path.GetLength(), _segments, _path.Closed, _range, _scale, _fade, m_VStrategy);
            _pathPoints.TryReusePersistentArray(extrusionSettings.segments);
            _path.Populate(_pathPoints, _range, _segments);
            if (_processor)
            {
                _processor.Process(_pathPoints);
                
            }
            PopulateMeshData(data, _pathPoints, extrusionSettings, _parallel);
        }

        protected abstract void PopulateMeshData(UnityEngine.Mesh.MeshData data, NativeArray<RigidTransform> pathPoints, ExtrusionSettings extrusionSettings, bool parallel);
    }
}