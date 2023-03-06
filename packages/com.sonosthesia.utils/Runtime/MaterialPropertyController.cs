using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MaterialPropertyController<T>
{
    protected readonly Material _material;
    protected readonly string _name;
    protected readonly int _id;
    protected T _reference;
        
    protected MaterialPropertyController(Material material, string name)
    {
        _material = material;
        _name = name;
        _id = Shader.PropertyToID(name);
    }

    public void Initialize() => _reference = Get();

    public abstract void Set(T current); 
        
    public abstract T Get();

    public abstract void Modulate(float scale);
}

public class MaterialFloatPropertyController : MaterialPropertyController<float>
{
    public override float Get() => _material.GetFloat(_id);
    public override void Set(float val) => _material.SetFloat(_id, val);
    public override void Modulate(float scale) => _material.SetFloat(_id, _reference * scale);

    public MaterialFloatPropertyController(Material material, string name) : base(material, name)
    {
    }
}
    
public class MaterialColorPropertyController : MaterialPropertyController<Color>
{
    public override Color Get() => _material.GetColor(_id);
    public override void Set(Color val) => _material.SetColor(_id, val);
    public override void Modulate(float scale) => _material.SetColor(_id, _reference * scale);

    public MaterialColorPropertyController(Material material, string name) : base(material, name)
    {
    }
}
