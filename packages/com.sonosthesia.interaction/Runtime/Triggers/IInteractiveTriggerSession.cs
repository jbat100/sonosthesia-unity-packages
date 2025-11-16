namespace Sonosthesia.Interaction
{
    public interface IInteractiveTriggerSession<in TEvent>
    {
        void Start(TEvent e);
        void Update(TEvent e);
        void End(TEvent e, out float release);
    }
}