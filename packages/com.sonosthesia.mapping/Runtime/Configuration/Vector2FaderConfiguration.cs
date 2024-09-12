using UnityEngine;

namespace Sonosthesia.Mapping
{
    [CreateAssetMenu(fileName = "Vector2Fader", menuName = "Sonosthesia/Faders/Vector2Fader")]
    public class Vector2FaderConfiguration : FaderConfiguration<Vector2>
    {
        [SerializeField] private FloatFaderSettings _xSettings;
        [SerializeField] private FloatFaderSettings _ySettings;
        
        protected override Vector2 PerformFade(float t)
        {
            return new Vector2(_xSettings?.Fade(t) ?? 0f, _ySettings?.Fade(t) ?? 0f);
        }
    }
}