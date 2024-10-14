using System;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class CompoundNoisePathProcessorComponent : MonoBehaviour
    {
        [SerializeField] private CompoundNoisePathProcessor _processor;

        [Header("Settings")] 
        
        [SerializeField] private Noise4DType _noiseType;
        [SerializeField] private EaseType _easeType = EaseType.linear;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _displacement = 0.1f;
        [SerializeField] private float _frequency = 10f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _speed = 1f;
        
        private Guid _id;
        private float _time;

        protected virtual void OnEnable()
        {
            _id = Guid.NewGuid();
            _time = 0;
        }

        protected virtual void Update()
        {
            _time += Time.deltaTime * _speed;

            CompoundPathNoiseInfo info = new CompoundPathNoiseInfo(
                _noiseType, _displacement, _easeType, transform.position,
                _radius, _time, _offset, _frequency);
            
            _processor.Register(_id, info);
        }

        protected virtual void OnDisable()
        {
            _processor.Unregister(_id);
        }
    }
}