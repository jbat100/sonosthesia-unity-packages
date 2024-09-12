using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class Vector3FaderConfiguration : FaderConfiguration<Vector3>
    {
        [SerializeField] private FloatFaderSettings _xSettings;
        [SerializeField] private FloatFaderSettings _ySettings;
        [SerializeField] private FloatFaderSettings _zSettings;
        
        protected override Vector3 PerformFade(float t)
        {
            return new Vector3(_xSettings?.Fade(t) ?? 0f, _ySettings?.Fade(t) ?? 0f, _zSettings?.Fade(t) ?? 0f);
        }
    }
}