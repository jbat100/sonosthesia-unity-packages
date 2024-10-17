using System;
using Sonosthesia.Ease;
using Sonosthesia.Noise;
using Unity.Mathematics;
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

        [Header("Local")] 
        
        [SerializeField] private bool _falloff;
        [SerializeField] private EaseType _falloffType;
        [SerializeField] private float _radius = 1f;

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
            float3 center = transform.position;
            
            CompoundMeshNoiseInfo info = new CompoundMeshNoiseInfo(
                _crossFadeType, _noiseType, 
                _displacement, _domainTRS.Matrix,
                _falloff, _falloffType, center, _radius,
                _time, _frequency
                );
            
            _controller.Register(_id, info);
        }

        protected virtual void OnDisable()
        {
            _controller.Unregister(_id);
        }
    }
}