using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDIExtractorSettings
    {
        public enum ExtractorType
        {
            Constant,
            Provider,
            Interactive
        }

        public enum Origin
        {
            Self,
            Source,
            Actor
        }
        
        [SerializeField] private ExtractorType _extractorType;
        public ExtractorType extractorType => _extractorType;
        
        [SerializeField] private Origin _origin = Origin.Self;
        public Origin origin => _origin;
    }
}