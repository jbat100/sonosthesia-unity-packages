using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public abstract class MIDINoteMaterialProvider : ObservableBehaviour
    {
        
        
        public abstract Material MaterialForNote(int note);
    }
}