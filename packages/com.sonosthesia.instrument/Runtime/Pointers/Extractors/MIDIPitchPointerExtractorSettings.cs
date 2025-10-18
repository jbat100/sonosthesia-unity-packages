using System;
using Sonosthesia.Pointer;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDIPitchPointerExtractorSettings : MIDIPitchExtractorSettings<PointerEvent>
    {
        // source could be target object
        protected override GameObject GetSource(PointerEvent e) => e.Source.gameObject;
        
        // actor could be pointer for example in the case of XR hand/controller pointer
        protected override GameObject GetActor(PointerEvent e) => null;
    }
}