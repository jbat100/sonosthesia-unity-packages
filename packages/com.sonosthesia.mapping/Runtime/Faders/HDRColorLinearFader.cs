using UnityEngine;

namespace Sonosthesia.Mapping
{
    /// <summary>
    /// Cannot inherit from LinearFader as we need the color usage attribute for _start and _end
    /// Could probably be done with a custom editor
    /// </summary>
    public class HDRColorLinearFader : Fader<Color>
    {
        [SerializeField, ColorUsage(true, true)] private Color _start;

        [SerializeField, ColorUsage(true, true)] private Color _end;

        [SerializeField] private bool _clamp;
        
        public override Color Fade(float fade)
        {
            Color result = Color.LerpUnclamped(_start, _end, _clamp ? Mathf.Clamp01(fade) : fade);
            //Debug.Log($"{this} lerped at {fade} from {_start} to {_end} with result {result}");
            return result;
        }
    }
}