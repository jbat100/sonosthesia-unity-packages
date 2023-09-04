using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public enum TargetBlend
    {
        None,
        Override,
        Add
    }

    public static class TargetBlendExtensions
    {
        public static float Blend(this TargetBlend targetBlend, float first, float second)
        {
            return targetBlend switch
            {
                TargetBlend.Override => second,
                TargetBlend.Add => first + second,
                _ => 0f
            };
        }
        
        public static Vector3 Blend(this TargetBlend targetBlend, Vector3 first, Vector3 second)
        {
            return targetBlend switch
            {
                TargetBlend.Override => second,
                TargetBlend.Add => first + second,
                _ => Vector3.zero
            };
        }
        
        public static Quaternion Blend(this TargetBlend targetBlend, Quaternion first, Quaternion second)
        {
            return targetBlend switch
            {
                TargetBlend.Override => second,
                TargetBlend.Add => first * second,
                _ => Quaternion.identity
            };
        }
    }
    
    public abstract class Target<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private TargetBlend _targetBlend;
        protected TargetBlend TargetBlend => _targetBlend;

        [SerializeField] private Signal<T> _source;

        private IDisposable _subscription;

        protected virtual void Awake()
        {
            if (!_source)
            {
                _source = GetComponent<Signal<T>>();
            }
        }
        
        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _source.SignalObservable.Subscribe(Apply);
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected abstract void Apply(T value);
    }
}