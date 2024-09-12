using FMOD;
using FMODUnity;

namespace Sonosthesia.FMOD
{
    public class FMODLoudnessChained : FMODLoudness
    {
        private DSP _meterDSP;
        
        protected override void PerformCleanup(ChannelGroup channelGroup)
        {
            if (channelGroup.hasHandle() && _meterDSP.hasHandle())
            {
                // note : we don't own the instance channel group, it is not our business to release it
                channelGroup.removeDSP(_meterDSP);
            }
            
            if (_meterDSP.hasHandle())
            {
                _meterDSP.release();
                _meterDSP = default;
            }
        }
        
        protected override bool PerformTrySetup(ChannelGroup channelGroup)
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.LOUDNESS_METER, out _meterDSP);
            UnityEngine.Debug.LogWarning($"createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = channelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.TAIL, _meterDSP);
            UnityEngine.Debug.LogWarning($"addDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetLoudness(LoudnessSelector selector, out float loudness)
        {
            return selector.Extract(_meterDSP, out loudness);
        }
    }    
}


