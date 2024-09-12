using UnityEngine;

namespace Sonosthesia.Utils
{
    public class HSVColorTest : MonoBehaviour
    {
        [SerializeField, ColorUsage(true, true)] private Color _color;

        [SerializeField] private float _hueOffset;

        protected void OnValidate()
        {
            HSVColor hsv = _color.ToHSV();
            HSVColor offset = hsv;
            offset.h = (offset.h + _hueOffset) % 1f;
            Color result = offset.ToRGB(false);
            
            Debug.Log($"{this} testing {_color} converted to {hsv} offset is {_color - result}");
        }
    }
}