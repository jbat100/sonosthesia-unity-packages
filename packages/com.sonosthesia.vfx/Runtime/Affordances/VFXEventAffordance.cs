using System;
using Sonosthesia.Extractor;
using Sonosthesia.Interaction;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.VFX
{
    public class VFXEventAffordance<TEvent> : InteractionAffordance<TEvent> where TEvent : struct 
    {
        [Serializable]
        private class AttributeConfiguration
        {
            [SerializeField] private string _name;
            public string Name => _name;
        
            [SerializeField] private InterfaceReference<IStaticExtractor<TEvent, float>> _extractor;
            public IStaticExtractor<TEvent, float> Extractor => _extractor.Value;
        }
        
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private string _eventName;
        
        [SerializeField] private AttributeConfiguration[] _attributes;

        protected override void OnStartedStream(Guid id, TEvent e)
        {
            base.OnStartedStream(id, e);
            VFXEventAttribute eventAttribute = _visualEffect.CreateVFXEventAttribute();
            foreach (AttributeConfiguration attribute in _attributes)
            {
                if (attribute.Extractor.Extract(e, out float value))
                {
                    eventAttribute.SetFloat(attribute.Name, value);
                }
            }
            _visualEffect.SendEvent(_eventName, eventAttribute);
        }
    }
}