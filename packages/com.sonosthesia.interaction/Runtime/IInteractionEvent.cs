namespace Sonosthesia.Interaction
{
    public interface IInteractionEvent
    {
        float StartTime { get; }
        
        IInteractionEndpoint Source { get; }
        IInteractionEndpoint Actor { get; }
    }
}