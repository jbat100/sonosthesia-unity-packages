using System.Collections.Generic;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "SizeCurveGroupTransformerConfiguration", menuName = "Sonosthesia/Transformer/SizeCurveGroupTransformerConfiguration")]
    public class ScaleCurveGroupTransformerConfiguration : GroupTransformerConfiguration
    {
        [SerializeField] private AnimationCurve _curve;

        [SerializeField] private Vector3 _scale = Vector3.one;

        [SerializeField] private Axes _scaleAxes = Axes.Y;
        
        public override void Apply<T>(IEnumerable<T> targets)
        {
            foreach (T element in targets)
            {
                float curve = _curve.Evaluate(element.Offset);
                Vector3 scale = curve * _scale;
                element.Transform.localScale = element.Transform.localScale.SetAxes(scale, _scaleAxes);
            }
        }
    }
}