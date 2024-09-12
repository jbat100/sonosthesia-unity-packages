using FMOD;
using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public abstract class FMODDSPProcessor : FMODProcessor
    {
        public enum Insert
        {
            Head,
            Tail
        }

        [SerializeField] private Insert _insert;

        private DSP _dsp;
        protected DSP DSP => _dsp;

        protected abstract DSP_TYPE DSPType { get; }

        protected abstract bool InitializeParameters(DSP dsp);
        
        protected sealed override bool PerformTrySetup(ChannelGroup channelGroup)
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createDSPByType(DSPType, out _dsp);
            UnityEngine.Debug.LogWarning($"createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            InitializeParameters(_dsp);
            
            RuntimeManager.StudioSystem.flushCommands();
            
            int index = _insert == Insert.Head ? CHANNELCONTROL_DSP_INDEX.HEAD : CHANNELCONTROL_DSP_INDEX.TAIL;

            result = channelGroup.addDSP(index, _dsp);
            UnityEngine.Debug.LogWarning($"addDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
            
        }

        protected sealed override void PerformCleanup(ChannelGroup channelGroup)
        {
            if (channelGroup.hasHandle() && _dsp.hasHandle())
            {
                // note : we don't own the instance channel group, it is not our business to release it
                channelGroup.removeDSP(_dsp);
            }
            
            if (_dsp.hasHandle())
            {
                _dsp.disconnectAll(true, true);
                _dsp.release();
                _dsp = default;
            }
        }
    }
}