using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class SwitchAdaptor<TSource, TTarget> : MapAdaptor<TSource, TTarget>
        where TTarget : struct where TSource : struct
    {
        [Serializable]
        private class Option
        {
            [SerializeField] private TSource _source;
            public TSource Source => _source;

            [SerializeField] private TTarget _target;
            public TTarget Target => _target;
        }

        [SerializeField] private TTarget _fallback;

        [SerializeField] private List<Option> _options;

        protected override TTarget Map(TSource source)
        {
            foreach (Option option in _options)
            {
                if (option.Source.Equals(source))
                {
                    return option.Target;
                }
            }

            return _fallback;
        }
    }
}