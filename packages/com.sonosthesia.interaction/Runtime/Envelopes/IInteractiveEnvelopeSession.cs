namespace Sonosthesia.Interaction
{
    public interface IInteractiveEnvelopeSession<in TEvent>
    {
        void Start(TEvent e);
        void Update(TEvent e);
        void End(TEvent e, out float release);
        
        float Evaluate();
    }
}