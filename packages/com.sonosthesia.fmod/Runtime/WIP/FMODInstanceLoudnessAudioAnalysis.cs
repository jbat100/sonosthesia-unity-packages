using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sonosthesia.Audio;
using Sonosthesia.Signal;
using UnityEngine;
using Debug = UnityEngine.Debug;

// WIP : not functional

namespace Sonosthesia.FMOD
{
    public enum AudioAnalysisBand
    {
        None,
        Lows,
        Mids,
        Highs
    }

    internal abstract class BandLoudnessAnalysis : IDisposable
    {
        // group used to run dsp analysis
        private ChannelGroup _dspChannelGroup;
        // temp group which should be cleaned up on dispose
        private ChannelGroup _tempChannelGroup;
        
        private LoudnessSelector _loudnessSelector;
        
        private DSP _filterDSP;
        private DSP _loudnessDSP;
        private DSP _sendDSP;
        private DSP _returnDSP;

        protected DSP FilterDSP => _filterDSP;

        protected abstract RESULT CreateFilter(out DSP filterDSP);

        public bool GetVolumeDB(out float volume)
        {
            RESULT result;
            volume = -80;
            if (_loudnessSelector == LoudnessSelector.None)
            {
                if (!_filterDSP.hasHandle())
                {
                    return false;
                }
                
                result = _filterDSP.getMeteringEnabled(out _, out bool enabled);
                if (result != RESULT.OK || !enabled)
                {
                    Debug.LogError("Expected enabled output metering");
                    return false;
                }

                result = _filterDSP.getMeteringInfo(out _, out DSP_METERING_INFO outputInfo);
                if (result != RESULT.OK)
                {
                    return false;
                }

                if (outputInfo.numchannels == 0)
                {
                    return false;
                }

                volume = outputInfo.peaklevel[0];
                return true;
            }
            else
            {
                if (!_loudnessDSP.hasHandle())
                {
                    return false;
                }
                return _loudnessSelector.Extract(_loudnessDSP, out volume);   
            }
        }
        
        
        public bool TrySetup(EventInstance instance, LoudnessSelector loudnessSelector)
        {
            _loudnessSelector = loudnessSelector;
            return TrySetupWithSendReturn(instance);
        }

        private bool TrySetupWithSendReturn(EventInstance instance)
        {
            if (!instance.isValid())
            {
                UnityEngine.Debug.LogWarning($"Setup called with invalid handle");
                return false;
            }
            
            RESULT result;
            
            result = instance.getChannelGroup(out ChannelGroup channelGroup); 
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} getChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            _dspChannelGroup = channelGroup;
            
            result = channelGroup.getDSP(CHANNELCONTROL_DSP_INDEX.HEAD, out DSP headDSP); 
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} getDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.SEND, out _sendDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} send createDSPByType {DSP_TYPE.LOUDNESS_METER} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = channelGroup.addDSP(1, _sendDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} send addDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.RETURN, out _returnDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} return createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _returnDSP.getParameterInt((int)DSP_RETURN.ID, out int returnID);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} returnID getParameterInt {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _sendDSP.setParameterInt((int)DSP_SEND.RETURNID, returnID);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} returnID setParameterInt {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _sendDSP.setParameterFloat((int)DSP_SEND.LEVEL, 1f);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} level setParameterFloat {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            if ( CreateFilter(out _filterDSP) != RESULT.OK )
            {
                return false;
            }

            _filterDSP.setWetDryMix(1, 1, 0);

            result = _filterDSP.addInput(_returnDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} addInput headDSP result");
            if (result != RESULT.OK)
            {
                return false;
            }

            if (_loudnessSelector != LoudnessSelector.None)
            {
                result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.LOUDNESS_METER, out _loudnessDSP);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} createDSPByType {DSP_TYPE.LOUDNESS_METER} {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
            
                result = _loudnessDSP.addInput(_filterDSP);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} connect filter loudness {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }

                result = headDSP.addInput(_loudnessDSP, out DSPConnection connection);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} connect loudness head {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
                
                result = connection.setMix(0);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setMix {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
                
                result = _loudnessDSP.setActive(true);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setActive {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
            }
            else
            {
                result = headDSP.addInput(_filterDSP, out DSPConnection connection);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} connect loudness head {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
                
                result = _filterDSP.setMeteringEnabled(false, true);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} connect loudness head {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
                
                result = connection.setMix(0);
                UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setMix {result}");
                if (result != RESULT.OK)
                {
                    return false;
                }
            }
            
            result = _sendDSP.setActive(true);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _returnDSP.setActive(true);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _filterDSP.setActive(true);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetupWithSendReturn)} setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }

        private bool TrySetupWithParentChannelGroup(EventInstance instance)
        {
            if (!instance.isValid())
            {
                UnityEngine.Debug.LogWarning($"Setup called with invalid handle");
                return false;
            }
            
            RESULT result;

            result = FMODDSPUtils.CreateParentChannelGroup(" Audio Analysis", instance, out _dspChannelGroup);
            if (result != RESULT.OK)
            {
                return false;
            }

            _tempChannelGroup = _dspChannelGroup;
            
            result = CreateFilter(out _filterDSP);
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _dspChannelGroup.addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _filterDSP);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} addDSP {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            result = _filterDSP.setActive(true);
            UnityEngine.Debug.LogWarning($"{nameof(TrySetup)} setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = FMODDSPUtils.InsertDSP(_dspChannelGroup, DSP_TYPE.LOUDNESS_METER, CHANNELCONTROL_DSP_INDEX.HEAD, out _loudnessDSP);
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
        
        public void Dispose()
        {
            if (_dspChannelGroup.hasHandle())
            {
                if (_filterDSP.hasHandle())
                {
                    _dspChannelGroup.removeDSP(_filterDSP);    
                }
                
                if (_loudnessDSP.hasHandle())
                {
                    _dspChannelGroup.removeDSP(_loudnessDSP);    
                }

                if (_sendDSP.hasHandle())
                {
                    _dspChannelGroup.removeDSP(_sendDSP);    
                }
            }

            if (_filterDSP.hasHandle())
            {
                _filterDSP.disconnectAll(true, true);
                _filterDSP.release();
                _filterDSP = default;
            }
            
            if (_loudnessDSP.hasHandle())
            {
                _loudnessDSP.disconnectAll(true, true);
                _loudnessDSP.release();
                _loudnessDSP = default;
            }
            
            if (_sendDSP.hasHandle())
            {
                _sendDSP.disconnectAll(true, true);
                _sendDSP.release();
                _sendDSP = default;
            }
            
            if (_returnDSP.hasHandle())
            {
                _returnDSP.disconnectAll(true, true);
                _returnDSP.release();
                _returnDSP = default;
            }

            if (_tempChannelGroup.hasHandle())
            {
                _tempChannelGroup.release();
                _tempChannelGroup = default;
            }
        }
    }

    internal abstract class SimpleBandLoudnessAnalysis : BandLoudnessAnalysis
    {
        private float _cutoff;
        public float Cutoff
        {
            get => _cutoff;
            set
            {
                if (Math.Abs(value - _cutoff) > 1e-3)
                {
                    _cutoff = value;
                    ApplyCutoff();
                }
            }
        }

        public SimpleBandLoudnessAnalysis(float cutoff)
        {
            _cutoff = cutoff;
        }

        protected abstract DSP_TYPE DSPType { get; }
        protected abstract int CutoffParameterIndex { get; }
        
        protected override RESULT CreateFilter(out DSP filterDSP)
        {
            RESULT result = RuntimeManager.CoreSystem.createDSPByType(DSPType, out filterDSP);
            UnityEngine.Debug.LogWarning($"{this} createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return ApplyCutoff();
        }

        private RESULT ApplyCutoff()
        {
            DSP dsp = FilterDSP;

            RESULT result = dsp.setParameterFloat(CutoffParameterIndex, _cutoff);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }

    }
    
    internal class SimpleLowBandLoudnessAnalysis : SimpleBandLoudnessAnalysis
    {
        public SimpleLowBandLoudnessAnalysis(float cutoff = 500f) : base(cutoff)
        {
        }

        protected override DSP_TYPE DSPType => DSP_TYPE.LOWPASS_SIMPLE;
        protected override int CutoffParameterIndex => (int)DSP_LOWPASS_SIMPLE.CUTOFF;
    }
    
    internal class SimpleHighBandLoudnessAnalysis : SimpleBandLoudnessAnalysis
    {
        public SimpleHighBandLoudnessAnalysis(float cutoff = 5000f) : base(cutoff)
        {
        }
        
        protected override DSP_TYPE DSPType => DSP_TYPE.HIGHPASS_SIMPLE;
        protected override int CutoffParameterIndex => (int)DSP_HIGHPASS_SIMPLE.CUTOFF;
    }

    internal class EQBandLoudnessAnalysis : BandLoudnessAnalysis
    {
        private const float PASS_GAIN = 0;
        private const float BLOCK_GAIN = -80;
        
        private readonly AudioAnalysisBand _band;
        private readonly DSP_THREE_EQ_CROSSOVERSLOPE_TYPE _crossoverSlope;

        public EQBandLoudnessAnalysis(AudioAnalysisBand band, DSP_THREE_EQ_CROSSOVERSLOPE_TYPE crossoverSlope, float lowCrossover, float highCrossover)
        {
            _band = band;
            _crossoverSlope = crossoverSlope;
            _lowCrossover = lowCrossover;
            _highCrossover = highCrossover;
        }
        
        private float _lowCrossover = 500;
        public float LowCrossover
        {
            get => _lowCrossover;
            set
            {
                if (Math.Abs(value - _lowCrossover) > 1e-3)
                {
                    _lowCrossover = value;
                    ApplyLowCrossover(FilterDSP);
                }
            }
        }
        
        private float _highCrossover = 5000;
        public float HighCrossover
        {
            get => _highCrossover;
            set
            {
                if (Math.Abs(value - _highCrossover) > 1e-3)
                {
                    _highCrossover = value;
                    ApplyHighCrossover(FilterDSP);
                }
            }
        }

        private RESULT ApplyLowCrossover(DSP dsp)
        {
            RESULT result = dsp.setParameterFloat((int)DSP_THREE_EQ.LOWCROSSOVER, _lowCrossover);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.LOWCROSSOVER} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }
        
        private RESULT ApplyHighCrossover(DSP dsp)
        {
            RESULT result = dsp.setParameterFloat((int)DSP_THREE_EQ.HIGHCROSSOVER, _highCrossover);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.HIGHCROSSOVER} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }
        
        protected override RESULT CreateFilter(out DSP filterDSP)
        {
            RESULT result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.THREE_EQ, out filterDSP);
            UnityEngine.Debug.LogWarning($"{this} createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return result;
            }
            
            if (ApplyLowCrossover(filterDSP) != RESULT.OK)
            {
                return result;
            }
            
            if (ApplyHighCrossover(filterDSP) != RESULT.OK)
            {
                return result;
            }
            
            result = filterDSP.setParameterInt((int)DSP_THREE_EQ.CROSSOVERSLOPE, (int)_crossoverSlope);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.CROSSOVERSLOPE} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            float lowGain = _band == AudioAnalysisBand.Lows ? PASS_GAIN : BLOCK_GAIN;
            float midGain = _band == AudioAnalysisBand.Mids ? PASS_GAIN : BLOCK_GAIN;
            float highGain = _band == AudioAnalysisBand.Highs ? PASS_GAIN : BLOCK_GAIN;

            UnityEngine.Debug.LogWarning($"{this} setup for band {_band}");
            
            result = filterDSP.setParameterFloat((int)DSP_THREE_EQ.LOWGAIN, lowGain);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.LOWGAIN} {lowGain} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }
            
            result = filterDSP.setParameterFloat((int)DSP_THREE_EQ.MIDGAIN, midGain);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.MIDGAIN} {midGain} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }
            
            result = filterDSP.setParameterFloat((int)DSP_THREE_EQ.HIGHGAIN, highGain);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_THREE_EQ.HIGHGAIN} {highGain} {result}");
            if (result != RESULT.OK)
            {
                return result;
            }

            return RESULT.OK;
        }
    }
    
    public class FMODInstanceLoudnessAudioAnalysis : FMODInstanceProcessor
    {
        [SerializeField] private Signal<ContinuousAnalysis> _target;

        [SerializeField] private LoudnessSelector _selector = LoudnessSelector.Momentary;

        [SerializeField] private DSP_THREE_EQ_CROSSOVERSLOPE_TYPE _crossoverSlope = DSP_THREE_EQ_CROSSOVERSLOPE_TYPE._12DB;
        
        [SerializeField] private float _lowCrossover = 500;

        [SerializeField] private float _highCrossover = 5000;

        private float _startTime;
        private BandLoudnessAnalysis _lowBandAnalysis;
        private BandLoudnessAnalysis _midBandAnalysis;
        private BandLoudnessAnalysis _highBandAnalysis;

        protected virtual void OnValidate()
        {
            if (_lowBandAnalysis is EQBandLoudnessAnalysis eqLowBandAnalysis)
            {
                eqLowBandAnalysis.LowCrossover = _lowCrossover;
                eqLowBandAnalysis.HighCrossover = _highCrossover;
            }
            
            if (_midBandAnalysis is EQBandLoudnessAnalysis eqMidBandAnalysis)
            {
                eqMidBandAnalysis.LowCrossover = _lowCrossover;
                eqMidBandAnalysis.HighCrossover = _highCrossover;
            }
            
            if (_highBandAnalysis is EQBandLoudnessAnalysis eqHighBandAnalysis)
            {
                eqHighBandAnalysis.LowCrossover = _lowCrossover;
                eqHighBandAnalysis.HighCrossover = _highCrossover;
            }
        }

        protected override bool TrySetup(EventInstance instance)
        {
            _startTime = Time.time;
            
            bool success = true;
            
            // Despite creating a new group the filter dsp is heard in the rendered audio
            // look into connection types:
            // https://www.fmod.com/docs/2.02/api/core-api-dspconnection.html#fmod_dspconnection_type

            _lowBandAnalysis = new EQBandLoudnessAnalysis(AudioAnalysisBand.Lows, _crossoverSlope, _lowCrossover, _highCrossover);
            success &= _lowBandAnalysis.TrySetup(instance, _selector);
            
            _midBandAnalysis = new EQBandLoudnessAnalysis(AudioAnalysisBand.Mids, _crossoverSlope, _lowCrossover, _highCrossover);
            success &= _midBandAnalysis.TrySetup(instance, _selector);
            
            _highBandAnalysis = new EQBandLoudnessAnalysis(AudioAnalysisBand.Highs, _crossoverSlope, _lowCrossover, _highCrossover);
            success &= _highBandAnalysis.TrySetup(instance, _selector);

            if (!success)
            {
                Cleanup();
            }
            
            return success;

        }

        protected override void Cleanup()
        {
            _lowBandAnalysis?.Dispose();
            _lowBandAnalysis = null;
            
            _midBandAnalysis?.Dispose();
            _midBandAnalysis = null;
            
            _highBandAnalysis?.Dispose();
            _highBandAnalysis = null;
        }

        protected override void Process()
        {
            if (!_target)
            {
                return;
            }
            
            float lows = 0, mids = 0, highs = 0;

            _lowBandAnalysis?.GetVolumeDB(out lows);
            _midBandAnalysis?.GetVolumeDB(out mids);
            _highBandAnalysis?.GetVolumeDB(out highs);

            float time = Time.time - _startTime;

            ContinuousAnalysis analysis = new ContinuousAnalysis
            {
                time = time,
                lows = lows,
                mids = mids,
                highs = highs
            };
            
            _target.Broadcast(analysis);
        }
    }
}