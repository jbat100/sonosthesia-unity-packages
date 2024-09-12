using FMOD;
using FMOD.Studio;

namespace Sonosthesia.FMOD
{
    public class FMODInstanceDebug : FMODInstanceProcessor
    {
        protected override bool TrySetup(EventInstance instance)
        {
            RESULT result = instance.getChannelGroup(out ChannelGroup group); 
            UnityEngine.Debug.LogWarning($"{this} getChannelGroup {group.handle} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = group.getNumDSPs(out int numDSPs);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} getNumDSPs {numDSPs}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }

        protected override void Cleanup()
        {
        }
    }
}