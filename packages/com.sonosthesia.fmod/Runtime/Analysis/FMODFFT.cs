using System;
using System.Runtime.InteropServices;
using FMOD;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODFFT : FMODDSPProcessor
    {
        [SerializeField] private DSP_FFT_WINDOW _windowType;

        [SerializeField] private int numberOfSamples = 1024;

        public int NumberOfSamples => numberOfSamples;

        public bool GetSpectrumData(float[] spectrum, int channel)
        {
            DSP dsp = DSP;
            
            if (!IsSetup || !dsp.hasHandle())
            {
                return false;
            }

            RESULT result = dsp.getParameterData((int)DSP_FFT.SPECTRUMDATA, out IntPtr unmanagedData, out uint length);
            if (result != RESULT.OK)
            {
                UnityEngine.Debug.LogWarning($"_fftDSP getParameterData {result}");
                return false;
            }
                
            DSP_PARAMETER_FFT fftData = (DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(DSP_PARAMETER_FFT));
            if (fftData.numchannels <= channel)
            {
                UnityEngine.Debug.LogWarning($"fftData numchannels {fftData.numchannels}");
                return false;
            }

            fftData.getSpectrum(channel, ref spectrum);
            
            return true;
        }

        protected override DSP_TYPE DSPType => DSP_TYPE.FFT;

        protected override bool InitializeParameters(DSP dsp)
        {
            RESULT result;
            
            result = dsp.setParameterInt((int)DSP_FFT.WINDOWTYPE, (int)_windowType);
            UnityEngine.Debug.LogWarning($"setParameterInt {DSP_FFT.WINDOWTYPE} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            result = dsp.setParameterInt((int)DSP_FFT.WINDOWSIZE, numberOfSamples * 2);
            UnityEngine.Debug.LogWarning($"setParameterInt {DSP_FFT.WINDOWSIZE} {result}");
            if (result != RESULT.OK)
            {
                return false;
            }

            return true;
        }
    }
}
