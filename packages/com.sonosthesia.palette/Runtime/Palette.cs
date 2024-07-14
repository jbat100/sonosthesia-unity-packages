using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Palette
{
    [CreateAssetMenu(fileName = "Palette", menuName = "Sonosthesia/Palette")]
    public class Palette : ScriptableObject
    {
        [SerializeField] private List<Color> _colors = new ();

        [SerializeField] private Gradient _gradient;

        public Color Evaluate(float value) => _gradient.Evaluate(value);
        
        protected virtual void OnValidate()
        {
            UpdateGradient();
        }

        internal void SetColors(IEnumerable<Color> colors)
        {
            _colors.Clear();
            _colors.AddRange(colors);
            UpdateGradient();
        }
        
        internal void UpdateGradient()
        {
            if (_colors == null || _colors.Count == 0)
                return;

            GradientColorKey[] colorKeys = new GradientColorKey[_colors.Count];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[_colors.Count];

            for (int i = 0; i < _colors.Count; i++)
            {
                colorKeys[i].color = _colors[i];
                colorKeys[i].time = (float)i / (_colors.Count - 1);
                alphaKeys[i].alpha = _colors[i].a;
                alphaKeys[i].time = (float)i / (_colors.Count - 1);
            }

            _gradient = new Gradient();
            _gradient.SetKeys(colorKeys, alphaKeys);
        }
    }
}