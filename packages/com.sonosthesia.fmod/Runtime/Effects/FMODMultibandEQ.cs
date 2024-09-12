using System;
using System.Collections.Generic;
using FMOD;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public enum FMODBandEQ
    {
        None,
        A,
        B,
        C,
        D,
        E
    }

    public enum FMODBandEQParameter
    {
        None,
        Gain,
        Frequency,
        Q
    }

    public class FMODMultibandEQ : FMODDSPProcessor
    {
        private readonly IReadOnlyList<FMODBandEQ> BANDS = new List<FMODBandEQ>()
        {
            FMODBandEQ.A, FMODBandEQ.B, FMODBandEQ.C, FMODBandEQ.D, FMODBandEQ.E
        };
        
        private readonly IReadOnlyList<FMODBandEQParameter> PARAMETERS = new List<FMODBandEQParameter>()
        {
            FMODBandEQParameter.Frequency, FMODBandEQParameter.Gain, FMODBandEQParameter.Q
        };

        [Serializable]
        private class Parameters
        {
            public DSP_MULTIBAND_EQ_FILTER_TYPE filter;
            public float gain;
            [LogRange(20, 1000, 20000)] public float frequency;
            public float q;
        }

        [SerializeField] private Parameters _a;
        [SerializeField] private Parameters _b;
        [SerializeField] private Parameters _c;
        [SerializeField] private Parameters _d;
        [SerializeField] private Parameters _e;
        
        public bool SetParameter(FMODBandEQ band, FMODBandEQParameter parameter, float value)
        {
            DSP dsp = DSP;

            if (!dsp.hasHandle())
            {
                return false;
            }
            
            if (!GetParameterIndex(band, parameter, out int index))
            {
                return false;
            }
            
            RESULT result = DSP.setParameterFloat(index, value);
            UnityEngine.Debug.LogWarning($"{nameof(Apply)} setParameterFloat frequency {index} {value} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            return true;
        }

        protected virtual void OnValidate()
        {
            ApplyAll(DSP);
        }

        protected override DSP_TYPE DSPType => DSP_TYPE.MULTIBAND_EQ;

        protected override bool InitializeParameters(DSP dsp)
        {
            return ApplyAll(dsp);
        }

        private bool ApplyAll(DSP dsp)
        {
            bool result = true;
            result &= Apply(dsp, FMODBandEQ.A, _a);
            result &= Apply(dsp, FMODBandEQ.B, _b);
            result &= Apply(dsp, FMODBandEQ.C, _c);
            result &= Apply(dsp, FMODBandEQ.D, _d);
            result &= Apply(dsp, FMODBandEQ.E, _e);
            return result;
        }
        
        private static bool Apply(DSP dsp, FMODBandEQ band, Parameters parameters)
        {
            if (!Application.isPlaying)
            {
                return false;
            }
            
            if (!dsp.hasHandle())
            {
                return false;
            }
            
            if (band == FMODBandEQ.None)
            {
                return false;
            }

            RESULT result;
            
            result = dsp.setParameterInt(GetFilterParameter(band), (int)parameters.filter);
            UnityEngine.Debug.LogWarning($"{nameof(Apply)} setParameterInt filter {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = dsp.setParameterFloat(GetFrequencyParameter(band), parameters.frequency);
            UnityEngine.Debug.LogWarning($"{nameof(Apply)} setParameterFloat frequency {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = dsp.setParameterFloat(GetGainParameter(band), parameters.gain);
            UnityEngine.Debug.LogWarning($"{nameof(Apply)} setParameterFloat gain {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = dsp.setParameterFloat(GetQParameter(band), parameters.q);
            UnityEngine.Debug.LogWarning($"{nameof(Apply)} setParameterFloat q {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }

        private static bool GetParameterIndex(FMODBandEQ band, FMODBandEQParameter parameter, out int index)
        {
            index = -1;
            if (band == FMODBandEQ.None || parameter == FMODBandEQParameter.None)
            {
                return false;
            }

            index = parameter switch
            {
                FMODBandEQParameter.Gain => GetGainParameter(band),
                FMODBandEQParameter.Frequency => GetFrequencyParameter(band),
                FMODBandEQParameter.Q => GetQParameter(band),
                _ => -1
            };

            return index != -1;
        }
        
        private static int GetFilterParameter(FMODBandEQ band) => band switch
        {
            FMODBandEQ.A => (int)DSP_MULTIBAND_EQ.A_FILTER,
            FMODBandEQ.B => (int)DSP_MULTIBAND_EQ.B_FILTER,
            FMODBandEQ.C => (int)DSP_MULTIBAND_EQ.C_FILTER,
            FMODBandEQ.D => (int)DSP_MULTIBAND_EQ.D_FILTER,
            FMODBandEQ.E => (int)DSP_MULTIBAND_EQ.E_FILTER,
            _ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
        };
        
        private static int GetFrequencyParameter(FMODBandEQ band) => band switch
        {
            FMODBandEQ.A => (int)DSP_MULTIBAND_EQ.A_FREQUENCY,
            FMODBandEQ.B => (int)DSP_MULTIBAND_EQ.B_FREQUENCY,
            FMODBandEQ.C => (int)DSP_MULTIBAND_EQ.C_FREQUENCY,
            FMODBandEQ.D => (int)DSP_MULTIBAND_EQ.D_FREQUENCY,
            FMODBandEQ.E => (int)DSP_MULTIBAND_EQ.E_FREQUENCY,
            _ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
        };
        
        private static int GetGainParameter(FMODBandEQ band) => band switch
        {
            FMODBandEQ.A => (int)DSP_MULTIBAND_EQ.A_GAIN,
            FMODBandEQ.B => (int)DSP_MULTIBAND_EQ.B_GAIN,
            FMODBandEQ.C => (int)DSP_MULTIBAND_EQ.C_GAIN,
            FMODBandEQ.D => (int)DSP_MULTIBAND_EQ.D_GAIN,
            FMODBandEQ.E => (int)DSP_MULTIBAND_EQ.E_GAIN,
            _ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
        };
        
        private static int GetQParameter(FMODBandEQ band) => band switch
        {
            FMODBandEQ.A => (int)DSP_MULTIBAND_EQ.A_Q,
            FMODBandEQ.B => (int)DSP_MULTIBAND_EQ.B_Q,
            FMODBandEQ.C => (int)DSP_MULTIBAND_EQ.C_Q,
            FMODBandEQ.D => (int)DSP_MULTIBAND_EQ.D_Q,
            FMODBandEQ.E => (int)DSP_MULTIBAND_EQ.E_Q,
            _ => throw new ArgumentOutOfRangeException(nameof(band), band, null)
        };
    }
}