using System;
using Sonosthesia.Touch;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [Serializable]
    public class MIDIChannelTouchExtractorSettings : MIDIChannelExtractorSettings<TouchEvent>
    {
        protected override GameObject GetSource(TouchEvent e) => e.touchData.Source.gameObject;
        protected override GameObject GetActor(TouchEvent e) => e.touchData.Actor.gameObject;
    }
}