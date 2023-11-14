using System.Collections.Generic;
using Sonosthesia.Utils;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseConfiguration : MonoBehaviour
    {
        [SerializeField] private List<DynamicNoiseSettings> _settings;

        [SerializeField] private int _seed;

        [SerializeField] private float _displacement = 1f;

        private bool _dirty = true;
        
        public int Count => _settings.Count;
        
        public DynamicNoiseSettings GetSettings(int index)
        {
            return _settings.GetIndexOrDefault(index);
        }
        
        private NativeArray<TriNoise.DomainNoiseComponent> _noiseComponents;
        private float[] _localTimes;

        public NativeArray<TriNoise.DomainNoiseComponent> NoiseComponents
        {
            get
            {
                if (!_dirty)
                {
                    return _noiseComponents;
                }
                
                for (int i = 0; i < Count; i++)
                {
                    DynamicNoiseSettings settings = GetSettings(i);
                    _noiseComponents[i] = new TriNoise.DomainNoiseComponent(
                        TriNoise.GetNoiseComponent(settings, _seed, _displacement, _localTimes[i]),
                        settings.Domain.Matrix,
                        settings.Domain.DerivativeMatrix
                    );
                }

                _dirty = false;
                return _noiseComponents;
            }
        }
        
        protected virtual void Update()
        {
            for (int i = 0; i < Count; i++)
            {
                _localTimes[i] += Time.deltaTime * GetSettings(i).Velocity;
            }
            _dirty = true;
        }
        
        protected virtual void OnEnable()
        {
            for (int i = 0; i < _localTimes.Length; i++)
            {
                _localTimes[i] = 0;
            }

            _dirty = true;
            CheckArrays();
        }

        protected virtual void OnValidate()
        {
            _dirty = true;
            CheckArrays();
        }

        protected virtual void OnDestroy()
        {
            _noiseComponents.Dispose();
        }
        
        private void CheckArrays()
        {
            int count = Count;
            if (_noiseComponents.Length != count)
            {
                _noiseComponents.Dispose();
                _noiseComponents = new NativeArray<TriNoise.DomainNoiseComponent>(count, Allocator.Persistent);
            }

            if (_localTimes == null || _localTimes.Length != count)
            {
                _localTimes = new float[count];
            }
        }
    }
}