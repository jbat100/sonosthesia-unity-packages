using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace Sonosthesia.FMOD
{
    // Misnomer, creating another "dsp" channel group and adding the "instance" channel group
    // to it does not create a side chain, it feeds the instance group into the dsp group 
    // which then bubbles up to master in a seemingly inevitable way
    
    public class FMODLoudnessSidechained : FMODLoudness
    {
        private ChannelGroup _dspChannelGroup;
        private DSP _loudnessDSP;

        protected override void PerformCleanup(ChannelGroup channelGroup)
        {
            if (_dspChannelGroup.hasHandle() && _loudnessDSP.hasHandle())
            {
                _dspChannelGroup.removeDSP(_loudnessDSP);
            }

            if (_dspChannelGroup.hasHandle())
            {
                _dspChannelGroup.release();
                _dspChannelGroup = default;
            }

            if (_loudnessDSP.hasHandle())
            {
                _loudnessDSP.release();
                _loudnessDSP = default;
            }
        }
        
        protected override bool PerformTrySetup(ChannelGroup channelGroup)
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createChannelGroup("Parrallel DSP", out _dspChannelGroup);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} createChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.LOUDNESS_METER, out _loudnessDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _dspChannelGroup.addGroup(channelGroup); 
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} getChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _dspChannelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.TAIL, _loudnessDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} addDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _loudnessDSP.setActive(true);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetLoudness(LoudnessSelector selector, out float loudness)
        {
            return selector.Extract(_loudnessDSP, out loudness);
        }
    }
}