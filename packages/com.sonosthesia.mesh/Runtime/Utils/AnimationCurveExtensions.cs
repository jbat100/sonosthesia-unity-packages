using Sonosthesia.Utils;
using Unity.Collections;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    public static class AnimationCurveExtensions
    {
        public static void Populate(this AnimationCurve animationCurve, NativeArray<float> curvePoints)
        {
            int count = curvePoints.Length;
            if (count < 2)
            {
                return;
            }
            float step = animationCurve.Duration() / (count - 1);
            float current = 0f;
            for (int i = 0; i < count; ++i, current += step)
            {
                curvePoints[i] = animationCurve.Evaluate(current);
            }
        }
    }
}