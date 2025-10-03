using Sonosthesia.Interaction;
using Sonosthesia.Trigger;

namespace Sonosthesia.Touch
{
    
    public static class TouchEnvelopeSessionUtil
    {
        public static IInteractiveEnvelopeSession<TouchEvent> StartSession(TouchEvent e, 
            TouchEnvelopeSettings settings,
            TriggerController controller = null)
        {
            IInteractiveEnvelopeSession<TouchEvent> session = InteractiveEnvelopeSessionUtil.MakeSession(settings.Type, settings, controller);

            if (settings.Filter == TouchEnvelopeSettings.FilterType.OneEuro)
            {
                session = new StaticInteractiveEnvelopeSessionOneEuroFilter<TouchEvent>(session, settings.OneEuroFilter);
            }
            
            session.Start(e);
            return session;
        }
    }
}