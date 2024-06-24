using System.Collections.Generic;
using FMOD;
using FMODUnity;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODAdditiveSynth : MonoBehaviour
    {
        [SerializeField] private List<FMODSynthSource> _sources;
        
        [SerializeField] private List<FMODProcessor> _processors;

        [SerializeField] private float _gain;
        
        private ChannelGroup _channelGroup;
        private DSP _faderDSP;

        protected virtual void OnEnable()
        {
            CleanupSynth();
            if (!SetupSynth())
            {
                CleanupSynth();
            }
        }

        protected virtual void OnDisable()
        {
            CleanupSynth();
        }

        protected virtual bool SetupSynth()
        {
            RESULT result;
            
            result = RuntimeManager.CoreSystem.createChannelGroup("Additive Synth", out _channelGroup);
            UnityEngine.Debug.LogWarning($"{this} createChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _channelGroup.getDSP(CHANNELCONTROL_DSP_INDEX.TAIL, out _faderDSP);
            UnityEngine.Debug.LogWarning($"{this} getDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _faderDSP.getType(out DSP_TYPE dspType);
            UnityEngine.Debug.LogWarning($"{this} getType {dspType} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            if (dspType != DSP_TYPE.FADER)
            {
                UnityEngine.Debug.LogError($"{this} unexpected tail type {dspType}");
                return false;
            }

            foreach (FMODProcessor processor in _processors)
            {
                if (!processor.TrySetup(_channelGroup))
                {
                    return false;
                }
            }
            
            result = _channelGroup.getDSP(CHANNELCONTROL_DSP_INDEX.TAIL, out DSP tailDSP);
            UnityEngine.Debug.LogWarning($"{this} getDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            foreach (FMODSynthSource source in _sources)
            {
                if (!source.CreateSource(tailDSP))
                {
                    return false;
                }
            }

            return true;
        }
        
        protected virtual void CleanupSynth()
        {
            foreach (FMODSynthSource source in _sources)
            {
                source.Cleanup();
            }
            
            foreach (FMODProcessor processor in _processors)
            {
                processor.Cleanup();
            }

            if (_channelGroup.hasHandle())
            {
                if (_faderDSP.hasHandle())
                {
                    _channelGroup.removeDSP(_faderDSP);
                }
                _channelGroup.release();
                _channelGroup.clearHandle();
            }

            if (_faderDSP.hasHandle())
            {
                _faderDSP.release();
                _faderDSP.clearHandle();
            }
        }
        
    }
}