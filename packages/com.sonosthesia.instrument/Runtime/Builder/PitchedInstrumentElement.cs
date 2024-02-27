using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentElement : GroupTransformerElement, IMIDIPitchedElement
    {
        public int MIDINote { get; set; }
        
        private Renderer _renderer;
        public Renderer Renderer
        {
            get
            {
                if (_renderer)
                {
                    return _renderer;
                }

                _renderer = GetComponentInChildren<Renderer>();
                return _renderer;
            }
        }
    }
}