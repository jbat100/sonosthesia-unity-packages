using System;
using Sonosthesia.Ease;
using Sonosthesia.Noise;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class CompoundNoiseMeshComponent : MonoBehaviour
    {
        [SerializeField] private CompoundNoiseMeshController _controller;

        [Header("Settings")] 
        
        [SerializeField] private CatlikeNoiseType _noiseType;
        [SerializeField] private float _displacement = 0.1f;
        [SerializeField] private int _frequency;
        [SerializeField] private SpaceTRS _domainTRS = new () { scale = 1f };
        [SerializeField] private EaseType _crossFadeType = EaseType.easeInOutQuint;
        [SerializeField] private float _speed = 1f;

        [Header("Falloff")] 
        
        [SerializeField] private bool _falloff;
        [SerializeField] private EaseType _falloffEase;
        [SerializeField] private SpatialFalloffShape _falloffShape;
        [SerializeField] private float _falloffRadius = 1f;
        [SerializeField] private Vector3 _falloffDirection = Vector3.up;

        private readonly Guid _id = Guid.NewGuid();
        private float _time;

        protected virtual void OnEnable()
        {
            _time = 0f;
        }
        
        protected virtual void Update()
        {
            if (!_controller)
            {
                return;
            }
            
            _time += Time.deltaTime * _speed;
            
            Vector3 center = transform.position;
            Vector3 handle = center + transform.TransformVector(_falloffDirection);

            SpatialFalloffInfo falloffInfo = new SpatialFalloffInfo(_falloff, _falloffShape, _falloffEase, 
                center, handle, _falloffRadius);
            
            CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                _crossFadeType, _noiseType, _displacement, _domainTRS.Matrix, falloffInfo, _time, _frequency);
            
            _controller.Register(_id, info);
        }

        protected virtual void OnDisable()
        {
            _controller.Unregister(_id);
        }
    }
}