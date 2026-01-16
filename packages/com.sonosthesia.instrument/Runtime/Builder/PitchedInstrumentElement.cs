using Sonosthesia.Scaffold;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentElement : GroupTransformerElement, IMIDIPitchProvider
    {
        public int MIDIPitch { get; set; }
        
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