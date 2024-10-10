using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    public interface IInteractionEndpoint
    {
        InteractionLayerMask InteractionLayers { get; }
    }
}