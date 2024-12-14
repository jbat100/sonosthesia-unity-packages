using System;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDIPitchTouchExtractorSettings
    {
        public enum ExtractorType
        {
            Custom,
            Static,
            Actor,
            Source
        }
        
        [SerializeField] private ExtractorType _extractorType;
        
        [SerializeField][Range(0, 127)] private int _value;

        [SerializeField] private MIDIPitchTouchExtractor _extractor;

        public bool TryExtract(TouchEvent touchEvent, out int val)
        {
            val = 0;
            switch (_extractorType)
            {
                case ExtractorType.Custom:
                    return _extractor && _extractor.TryExtract(touchEvent, out val);
                case ExtractorType.Static:
                    val = _value;
                    return true;
                case ExtractorType.Actor:
                    return TryExtract(touchEvent.touchData.Actor.GetComponent<IMIDIPitchProvider>(), out val);
                case ExtractorType.Source:
                    return TryExtract(touchEvent.touchData.Source.GetComponent<IMIDIPitchProvider>(), out val);
                default:
                    return false;
            }
        }

        private bool TryExtract(IMIDIPitchProvider provider, out int val)
        {
            if (provider != null)
            {
                val = provider.MIDIPitch;
                return true;
            }

            val = 0;
            return false;
        }
    }
}