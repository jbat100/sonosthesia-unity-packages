using UnityEngine;

namespace Sonosthesia.Mapping
{
    [CreateAssetMenu(fileName = "FloatFader", menuName = "Sonosthesia/Faders/FloatFader")]
    public class FloatFaderConfiguration : FaderConfiguration<float>
    {
        [SerializeField] private FloatFaderSettings _settings;
        
        protected override float PerformFade(float t) => _settings.Fade(t);
    }
}