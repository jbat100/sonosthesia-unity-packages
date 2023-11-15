using Sonosthesia.Mapping;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class FaderAdaptor<TTarget> : MapAdaptor<float, TTarget> where TTarget : struct
    {
        [SerializeField] private Fader<TTarget> _fader;

        protected override TTarget Map(float source) => _fader.Fade(source);
    }
}