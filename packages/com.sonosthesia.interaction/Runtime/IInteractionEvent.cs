namespace Sonosthesia.Interaction
{
    public interface IInteractionEvent
    {
        IInteractionEndpoint Source { get; }
        IInteractionEndpoint Actor { get; }
    }
}