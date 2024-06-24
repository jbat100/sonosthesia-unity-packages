using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace Sonosthesia.FMOD
{
    public static class FMODDSPUtils
    {
        private const int MAX_NAME_LENGTH = 100;
        
        public static RESULT CreateParentChannelGroup(string suffix, EventInstance instance, out ChannelGroup parent)
        {
            RESULT result;
            parent = default;
            
            result = instance.getChannelGroup(out ChannelGroup instanceChannelGroup); 
            UnityEngine.Debug.LogWarning($"{nameof(CreateParentChannelGroup)} getChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            result = instanceChannelGroup.getName(out string instanceChannelGroupName, MAX_NAME_LENGTH);
            UnityEngine.Debug.LogWarning($"{nameof(CreateParentChannelGroup)} getName {result}");
            if (result != RESULT.OK)
            {
                return result;
            }
            
            string name = instanceChannelGroupName + suffix; 

            return CreateParentChannelGroup(name, instanceChannelGroup, out parent);

        }
        
        // TODO : this does not create a side chain, the output of the created group is the master group channel
        // so the filtered signals are actually played
        
        public static RESULT CreateParentChannelGroup(string name, ChannelGroup input, out ChannelGroup parent)
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createChannelGroup(name, out parent);
            UnityEngine.Debug.LogWarning($"{nameof(CreateParentChannelGroup)} createChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return result;
            }
            
            result = parent.addGroup(input); 
            UnityEngine.Debug.LogWarning($"{nameof(CreateParentChannelGroup)} addGroup {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }

        public static RESULT InsertDSP(ChannelGroup group, DSP_TYPE dspType, int index, out DSP dsp)
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createDSPByType(dspType, out dsp);
            UnityEngine.Debug.LogWarning($"{nameof(InsertDSP)} createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            result = group.addDSP(index, dsp);
            UnityEngine.Debug.LogWarning($"{nameof(InsertDSP)} addDSP {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }
    }
}