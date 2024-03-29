using System;
using Sonosthesia.Mapping;
using UnityEngine;

namespace Sonosthesia.Link
{
    public abstract class FaderLinkMapper<TSource, TTarget> : LinkMapper<TSource, TTarget> where TSource : struct where TTarget : struct
    {
        private enum DriveType
        {
            None,
            Absolute,
            Difference
        }

        [SerializeField] private Fader<TTarget> _fader;

        [SerializeField] private DriveType _driveType;
        
        [SerializeField] private float _offset;
        
        [SerializeField] private float _scale = 1f;

        protected abstract float Drive(TSource payload);

        public override TTarget Map(TSource source, TSource reference, float timeOffset)
        {
            float drive = _driveType switch
            {
                DriveType.None => 0f,
                DriveType.Absolute => Drive(source),
                DriveType.Difference => Drive(source) - Drive(reference),
                _ => throw new ArgumentOutOfRangeException()
            };
            return _fader.Fade((drive * _scale) + _offset);
        }
    }
}