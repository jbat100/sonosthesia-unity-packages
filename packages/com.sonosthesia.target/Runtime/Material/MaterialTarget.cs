using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Target
{
    public abstract class MaterialTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private Renderer _renderer;

        [SerializeField] private string _name;

        protected string Name => _name;
        
        protected int NameID { get; private set; }

        protected virtual string DefaultName => "";
        
        protected override void Awake()
        {
            base.Awake();
            if (!_renderer)
            {
                _renderer = GetComponent<Renderer>();
            }
            if (string.IsNullOrEmpty(_name))
            {
                _name = DefaultName;
            }
            NameID = Shader.PropertyToID(_name);
        }

        protected sealed override void Apply(T value)
        {
            Apply(value, _renderer.material);
        }

        protected abstract void Apply(T value, Material material);

    }
}