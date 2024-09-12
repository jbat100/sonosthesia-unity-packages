using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class ConfigurableFader<T> : Fader<T> where T : struct
    {
        [SerializeField] private FaderConfiguration<T> _configuration;

        public override T Fade(float fade) => _configuration.Fade(fade);
    }
}