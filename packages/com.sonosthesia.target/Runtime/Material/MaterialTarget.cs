using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Target
{
    /// Note : consider also using Material.Lerp
    
    public enum MaterialSelector
    {
        None,
        Main,
        Shared,
        Indexed,
        All
    }
    
    public abstract class MaterialTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private string _name;
        
        [SerializeField] private Renderer _renderer;
        
        [SerializeField] private List<Renderer> _renderers;

        [SerializeField] private LODGroup _group;

        [SerializeField] private MaterialSelector _materialSelector = MaterialSelector.Main;
        
        [SerializeField] private bool _usePropertyBlock;

        [SerializeField] private int _materialIndex;

        private readonly List<Renderer> _cachedRenderers = new ();
        private readonly List<Material> _cachedMaterials = new ();
        private MaterialPropertyBlock _propertyBlock;
        
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
        }

        private void SetupCaches()
        {
            _cachedRenderers.Clear();
            _cachedMaterials.Clear();

            // We don't want to execute if in edit mode as:
            // - Signals don't fire in edit mode, also calling r.material
            // - while in edit mode causes material leak error messages
            
            if (!Application.isPlaying)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(_name))
            {
                _name = DefaultName;
            }
            NameID = Shader.PropertyToID(_name);
            
            
            if (_renderer)
            {
                _cachedRenderers.Add(_renderer);    
            }
            _cachedRenderers.AddRange(_renderers.Where(r => r));
            if (_group)
            {
                _cachedRenderers.AddRange(_group.GetLODs().SelectMany(lod => lod.renderers).Where(r => r));    
            }
            
            _cachedMaterials.Clear();

            if (_usePropertyBlock)
            {
                _propertyBlock = null;
                return;
            }

            _propertyBlock = new MaterialPropertyBlock();
            
            foreach (Renderer r in _cachedRenderers)
            {
                switch (_materialSelector)
                {
                    case MaterialSelector.None:
                        break;
                    case MaterialSelector.Main:
                        _cachedMaterials.Add(r.material);   
                        break;
                    case MaterialSelector.Shared:
                        _cachedMaterials.Add(r.sharedMaterial);   
                        break;
                    case MaterialSelector.Indexed:
                        if (_materialIndex < r.materials.Length)
                        {
                            _cachedMaterials.Add(r.materials[_materialIndex]);   
                        }
                        break;
                    case MaterialSelector.All:
                        _cachedMaterials.AddRange(r.materials);
                        break;
                }
            }

            foreach (Material material in _cachedMaterials)
            {
                if (!CheckTarget(material))
                {
                    Debug.LogError($"{this} {nameof(Material)} does not have target with name {_name}");
                }
            }
        }

        protected virtual void OnValidate()
        {
            SetupCaches();
        }
        
        protected override void OnEnable()
        {
            SetupCaches();
            base.OnEnable();
        }

        protected sealed override void Apply(T value)
        {
            if (_usePropertyBlock)
            {
                ConfigureBlock(value, _propertyBlock);
                foreach (Renderer r in _cachedRenderers)
                {
                    r.SetPropertyBlock(_propertyBlock);
                }
            }
            else
            {
                Debug.Log($"{this} {nameof(Apply)} {value} to {_cachedMaterials.Count} materials");
                foreach (Material m in _cachedMaterials)
                {
                    Apply(value, m);
                }
            }
        }

        protected abstract bool CheckTarget(Material material);
        
        protected abstract void Apply(T value, Material material);
        
        protected abstract void ConfigureBlock(T value, MaterialPropertyBlock block);

    }
}