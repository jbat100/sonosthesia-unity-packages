using FMOD;
using FMODUnity;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODOscillatorSynthSource : FMODSynthSource
    {
        // https://www.fmod.com/docs/2.02/api/core-api-common-dsp-effects.html#fmod_dsp_oscillator_type
        public enum Oscillator : int
        {
            Sine,
            Square,
            SawUp,
            SawDown,
            Triangle,
            Noise
        }

        [SerializeField] private Oscillator _oscillator;

        [SerializeField] [LogRange(20, 1000, 20000)] private float _rate;

        [SerializeField] [Range(0, 2)] private float _volume;

        private DSP _dsp;
        private DSPConnection _connection;
        
        public override bool CreateSource(DSP target)
        {
            RESULT result = RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.OSCILLATOR, out _dsp);
            UnityEngine.Debug.LogWarning($"createDSPByType {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = target.addInput(_dsp, out _connection);
            UnityEngine.Debug.LogWarning($"addInput {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            if (!Apply())
            {
                return false;
            }
            
            result = _dsp.setActive(true);
            UnityEngine.Debug.LogWarning($"setActive {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            return true;
        }

        public override void Cleanup()
        {
            if (_dsp.hasHandle())
            {
                _dsp.disconnectAll(true, true);
                _dsp.release();
                _dsp = default;
            }

            if (_connection.hasHandle())
            {
                // apparently has no release, handled by underlying lib
                _connection = default;
            }
        }

        protected override bool Apply()
        {
            if (!_dsp.hasHandle())
            {
                return false;  
            }
            
            RESULT result = _dsp.setParameterInt((int)DSP_OSCILLATOR.TYPE, (int)_oscillator);
            UnityEngine.Debug.LogWarning($"{this} setParameterInt {DSP_OSCILLATOR.TYPE} {_oscillator} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = _dsp.setParameterFloat((int)DSP_OSCILLATOR.RATE, _rate);
            UnityEngine.Debug.LogWarning($"{this} setParameterFloat {DSP_OSCILLATOR.RATE} {_rate} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            if (!_connection.hasHandle())
            {
                return false;
            }
            
            result = _connection.setMix(_volume);
            UnityEngine.Debug.LogWarning($"{this} setMix {_volume} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            return true;
        }
    }
}