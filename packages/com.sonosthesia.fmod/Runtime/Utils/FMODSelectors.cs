using System;
using System.Runtime.InteropServices;
using FMOD;

namespace Sonosthesia.FMOD
{
    public enum LoudnessSelector
    {
        None,
        Momentary,
        ShortTerm,
        Integrated,
        Percentile10,
        Percentile95,
        MaxTruePeak,
        MaxMomentary
    }

    public static class LoudnessSelectionExtensions
    {
        public static float Select(this ref DSP_LOUDNESS_METER_INFO_TYPE info, LoudnessSelector selector)
        {
            return selector switch
            {
                LoudnessSelector.Integrated => info.integratedloudness,
                LoudnessSelector.Momentary => info.momentaryloudness,
                LoudnessSelector.ShortTerm => info.shorttermloudness,
                LoudnessSelector.Percentile10 => info.loudness10thpercentile,
                LoudnessSelector.Percentile95 => info.loudness95thpercentile,
                LoudnessSelector.MaxTruePeak => info.maxtruepeak,
                LoudnessSelector.MaxMomentary => info.maxtruepeak,
                _ => 0
            };
        }

        /// <summary>
        /// Expects DSP to be DSP_TYPE.LOUDNESS_METER
        /// </summary>
        /// <param name="dsp"></param>
        /// <param name="selector"></param>
        /// <param name="loudness"></param>
        /// <returns></returns>
        public static bool Extract(this LoudnessSelector selector, DSP dsp, out float loudness)
        {
            if (!dsp.hasHandle() || selector == LoudnessSelector.None)
            {
                loudness = 0;
                return false;
            }

            // Get the metering data from the DSP meter
            dsp.getParameterData((int)DSP_LOUDNESS_METER.INFO, out IntPtr data, out uint length);

            // https://www.fmod.com/docs/2.01/api/core-api-common-dsp-effects.html#fmod_dsp_loudness_meter_info_type
            DSP_LOUDNESS_METER_INFO_TYPE info =
                (DSP_LOUDNESS_METER_INFO_TYPE)Marshal.PtrToStructure(data, typeof(DSP_LOUDNESS_METER_INFO_TYPE));

            loudness = info.Select(selector);
            return true;
        }
    }
}