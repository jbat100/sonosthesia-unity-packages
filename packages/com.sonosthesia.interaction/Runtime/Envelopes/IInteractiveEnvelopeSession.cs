namespace Sonosthesia.Interaction
{
    public interface IInteractiveEnvelopeSession<in TEvent> where TEvent : IInteractionEvent
    {
        void Start(TEvent e);
        void Update(TEvent e);
        void End(TEvent e, out float release);

        float Update();
    }
}