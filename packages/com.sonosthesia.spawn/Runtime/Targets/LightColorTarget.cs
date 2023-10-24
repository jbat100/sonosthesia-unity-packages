using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class LightColorTarget : Target<Color>
    {
        [SerializeField] private Light _light;

        protected override void Awake()
        {
            base.Awake();
            if (!_light)
            {
                _light = GetComponent<Light>();
            }
        }

        protected override void Apply(Color value)
        {
            _light.color = value;
        }
    }
}