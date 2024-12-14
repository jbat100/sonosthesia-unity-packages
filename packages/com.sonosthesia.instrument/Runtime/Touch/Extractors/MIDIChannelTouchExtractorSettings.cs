using System;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDIChannelTouchExtractorSettings
    {
        public enum ExtractorType
        {
            Custom,
            Static,
            Actor,
            Source
        }

        [SerializeField] private ExtractorType _extractorType;
        
        [SerializeField][Range(0, 15)] private int _channel;

        [SerializeField] private MIDIChannelTouchExtractor _extractor;

        public int Extract(TouchEvent touchEvent)
        {
            TryExtract(touchEvent, out int channel);
            return channel;
        }
        
        public bool TryExtract(TouchEvent touchEvent, out int channel)
        {
            channel = 0;
            switch (_extractorType)
            {
                case ExtractorType.Custom:
                    return _extractor && _extractor.TryExtract(touchEvent, out channel);
                case ExtractorType.Static:
                    channel = _channel;
                    return true;
                case ExtractorType.Actor:
                    return TryExtract(touchEvent.touchData.Actor.GetComponent<IMIDIChannelProvider>(), out channel);
                case ExtractorType.Source:
                    return TryExtract(touchEvent.touchData.Source.GetComponent<IMIDIChannelProvider>(), out channel);
                default:
                    return false;
            }
        }
        
        private bool TryExtract(IMIDIChannelProvider provider, out int channel)
        {
            if (provider != null)
            {
                channel = provider.MIDIChannel;
                return true;
            }

            channel = 0;
            return false;
        }
    }
}