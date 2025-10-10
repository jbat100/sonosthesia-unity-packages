using System;
using Sonosthesia.Ease;
using Sonosthesia.Envelope;
using Sonosthesia.Interaction;
using Sonosthesia.Trigger;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class TouchEnvelopeSettings : InteractiveEnvelopeSettings<TouchEvent, FloatDynamicExtractorSettings<TouchEvent>, FloatStaticExtractorSettings<TouchEvent>>
    {
       
    }

    public static class TrackedTouchEnvelopeSettingsExtensions
    {
        public static IInteractiveEnvelopeSession<TouchEvent> SetupSession(this TouchEnvelopeSettings settings, TouchEvent e,
            TriggerController controller = null)
        {
            return InteractiveEnvelopeSessionUtil.StartSession(e, settings, controller);
        }
    }
}