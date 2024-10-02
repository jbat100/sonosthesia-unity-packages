using UnityEngine;

namespace Sonosthesia.Generator
{
    public enum PrimitiveOscillationType
    {
        None,
        Constant,
        Square,
        Sine,
        Cosine,
        Triangle,
        Saw,
        Line
    }

    public static class PrimitiveOscillation
    {
        public static float Evaluate(PrimitiveOscillationType oscillationType, float time)
        {
            time %= 1f;
            return oscillationType switch
            {
                PrimitiveOscillationType.Square => (time <= 0.5f) ? 1f : 0f,
                PrimitiveOscillationType.Sine => Mathf.Sin(time * Mathf.PI * 2f) * 0.5f + 0.5f,
                PrimitiveOscillationType.Cosine => Mathf.Cos(time * Mathf.PI * 2f) * 0.5f + 0.5f,
                PrimitiveOscillationType.Triangle => (time <= 0.5f) ? time * 2f : 2f - time * 2f,
                PrimitiveOscillationType.Saw => time,
                PrimitiveOscillationType.Constant => 1f,
                _ => 0f
            };
        }
    }
    
    public class PrimitiveFloatOscillator : FloatOscillator
    {
        [SerializeField] private PrimitiveOscillationType _primitiveType;

        protected override float Duration => 1f;

        protected override float EvaluateIteration(float time) => PrimitiveOscillation.Evaluate(_primitiveType, time);
    }
}