using Sonosthesia.Utils.Editor;
using UnityEditor.UIElements;

namespace Sonosthesia.Processing.Editor
{
    public static class VisualElementExtensions
    {
        public static void Show(this FloatProcessingType processingType, PropertyField curve, PropertyField remap, PropertyField clamp, PropertyField randomization)
        {
            curve.Show(processingType.HasFlag(FloatProcessingType.Curve));
            remap.Show(processingType.HasFlag(FloatProcessingType.Remap));
            clamp.Show(processingType.HasFlag(FloatProcessingType.Clamp));
            randomization.Show(processingType.HasFlag(FloatProcessingType.Randomize));
        }
    }
}