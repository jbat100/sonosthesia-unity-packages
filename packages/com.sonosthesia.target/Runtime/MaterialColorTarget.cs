using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Target
{
    public class MaterialColorTarget : Target<Color>
    {
        [SerializeField] private Renderer _renderer;

        [SerializeField] private string _name = "_BaseColor";
        
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