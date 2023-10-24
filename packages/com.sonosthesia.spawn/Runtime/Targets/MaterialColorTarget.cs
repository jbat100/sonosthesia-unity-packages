using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class MaterialColorTarget : Target<Color>
    {
        [SerializeField] private Renderer _renderer;

        [SerializeField] private string _name;
        
        protected override void Awake()
        {
            base.Awake();
            if (!_renderer)
            {
                _renderer = GetComponent<Renderer>();
            }
        }

        protected override void Apply(Color value)
        {
            _renderer.material.SetColor(_name, value);
        }
    }
}