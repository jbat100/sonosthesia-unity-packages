using UnityEngine;

namespace Sonosthesia.Generator
{
    public class PrimitiveFloatOscillator : FloatOscillator
    {
        private enum PrimitiveType
        {
            None,
            Constant,
            Square,
            Sine,
            Triangle,
            Saw,
            Line
        }

        [SerializeField] private PrimitiveType _primitiveType;

        protected override float Duration() => 1f;

        protected override float EvaluateIteration(float time)
        {
            return _primitiveType switch
            {
                PrimitiveType.Square => (time <= 0.5f) ? 1f : 0f,
                PrimitiveType.Sine => Mathf.Sin(time * Mathf.PI * 2f) * 0.5f + 0.5f,
                PrimitiveType.Triangle => (time <= 0.5f) ? time * 2f : 2f - time * 2f,
                PrimitiveType.Saw => time,
                PrimitiveType.Constant => 1f,
                _ => 0f
            };
        }
    }
}