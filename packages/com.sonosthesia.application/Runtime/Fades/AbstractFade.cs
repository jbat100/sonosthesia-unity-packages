using UnityEngine;

namespace Sonosthesia.Application
{
    public abstract class AbstractFade : MonoBehaviour
    {
        private float _fade;
        public float Fade
        {
            get => _fade;
            set
            {
                _fade = value;
                Apply(value);
            }
        }

        protected abstract void Apply(float fade);
    }
}