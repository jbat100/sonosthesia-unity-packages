using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class MaterialColorTarget : Target<Color>
    {
        [SerializeField] private Renderer _renderer;

        [SerializeField] private string _name;
        
        protected void Awake()
        {
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