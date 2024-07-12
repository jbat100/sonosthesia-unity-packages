using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline
{
    // Used to mark proxy containing objects for automatic signal creation nested IProxyContainers are supported
    // and will result in nested hierarchy of GameObjects with FloatSignal components 
    // Also used for recursive field discovery for automatic update call
    public interface IProxyContainer
    {
            
    }
    
    public class AnimationProxy : MonoBehaviour, IProxyContainer
    {
        /// <summary>
        /// Subclasses can register for auto update which will use reflection to call Update on Proxy fields
        /// Current implementation uses boxing however, so best avoided
        /// </summary>
        protected virtual bool AutoUpdate => false;
        
        // Proxy must be a struct to allow for animator access
        // Tried to use ISerializationCallbackReceiver to intercept Timeline modifications but callbacks don't get called
        
        [Serializable]
        public struct Proxy
        {
            public float value;
            public Signal<float> signal;

            public void Update()
            {
                if (signal)
                {
                    signal.Broadcast(value);
                }
            }
        }
        
        private static readonly Dictionary<Type, List<FieldInfo>> typeToProxyFieldsCache = new ();
        
        private List<FieldInfo> _proxyFields = new ();
        
        protected virtual void Awake()
        {
            if (AutoUpdate)
            {
                _proxyFields = GetOrCacheProxyFields();   
            }
        }

        protected virtual void Update()
        {
            if (!AutoUpdate)
            {
                return;
            }
            // Debug.Log($"{this} auto updating {_proxyFields.Count} proxies");
            foreach (FieldInfo field in _proxyFields)
            {
                Proxy proxy = (Proxy)field.GetValue(this);
                proxy.Update();
            }
        }
        
        private List<FieldInfo> GetOrCacheProxyFields()
        {
            Type type = this.GetType();
            if (!typeToProxyFieldsCache.TryGetValue(type, out var fields))
            {
                fields = DiscoverProxyFields(this);
                typeToProxyFieldsCache[type] = fields;
            }
            return fields;
        }

        private List<FieldInfo> DiscoverProxyFields(object obj)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            DiscoverProxyFieldsRecursive(obj, fields);
            return fields;
        }

        private void DiscoverProxyFieldsRecursive(object obj, List<FieldInfo> fields)
        {
            if (obj == null)
            {
                return;
            }

            Type type = obj.GetType();

            while (type != null && type != typeof(MonoBehaviour))
            {
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fieldInfos)
                {
                    if (field.FieldType == typeof(Proxy))
                    {
                        fields.Add(field);
                    }
                    else if (typeof(IProxyContainer).IsAssignableFrom(field.FieldType))
                    {
                        // If the field is of a type that implements IProxyContainer, recursively discover its Proxy fields
                        object nestedObj = field.GetValue(obj);
                        DiscoverProxyFieldsRecursive(nestedObj, fields);
                    }
                }
                type = type.BaseType;
            }
        }
    }
}