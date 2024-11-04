using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.Interaction
{
    // when checking for compatibility between sources, actors and affordances, there are two different ways of checking
    // - Block: check always fails
    // - Pass: check always succeeds
    // - Any: other must have at least one layer in mask
    // - All: other must have all layers in mask
    // - Exact: other and mask must be exactly the same

    public enum InteractionLayerMatch
    {
        Block,
        Pass,
        Any,
        All,
        Exact
    }

    public static class InteractionLayerMatchExtensions
    {
        public static bool Match(this InteractionLayerMatch match, InteractionLayerMask layers, InteractionLayerMask other)
        {
            return match switch
            {
                InteractionLayerMatch.Pass => true,
                InteractionLayerMatch.Any => (layers & other) != 0,
                InteractionLayerMatch.All => (layers & other) == layers,
                InteractionLayerMatch.Exact => layers == other,
                _ => false
            };
        }
    }

}