using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class InterfaceReference<TInterface, TObject> where TObject : Object where TInterface : class 
{
    [SerializeField, HideInInspector] private TObject underlyingValue;

    public TInterface Value
    {
        get
        {
            if (!underlyingValue) return null;
            if (underlyingValue is TInterface i) return i;
            throw new InvalidOperationException(
                $"{underlyingValue} must implement {typeof(TInterface)}.");
        }
        set
        {
            if (value is null)
            {
                underlyingValue = null;
                return;
            }

            if (value is TObject t && value is TInterface)
            {
                underlyingValue = t;
                return;
            }

            throw new ArgumentException(
                $"{value} must be assignable to {typeof(TObject)} and implement {typeof(TInterface)}.",
                nameof(value));
        }
    }

    public TObject UnderlyingValue
    {
        get => underlyingValue;
        set
        {
            if (!value)
            {
                underlyingValue = null;
                return;
            }

            if (value is TInterface)
            {
                underlyingValue = value;
                return;
            }

            throw new ArgumentException(
                $"{value} must implement {typeof(TInterface)}.",
                nameof(value));
        }
    }

    public InterfaceReference() { }
    public InterfaceReference(TObject target)
    {
        UnderlyingValue = target; // runs validation
    }

    public InterfaceReference(TInterface @interface)
    {
        Value = @interface; // runs validation
    }

    public static implicit operator TInterface(InterfaceReference<TInterface, TObject> obj)
        => obj is null ? null : obj.Value;
}

[Serializable]
public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }