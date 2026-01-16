using Sonosthesia.Pack;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.PackAudio
{
    public class AudioQuintBandSplitter : AudioBandSplitter<PackedAudioQuintBands>
    {
        [SerializeField] private InterfaceReference<ISignal<float>> _b1;
        [SerializeField] private InterfaceReference<ISignal<float>> _b2;
        [SerializeField] private InterfaceReference<ISignal<float>> _b3;
        [SerializeField] private InterfaceReference<ISignal<float>> _b4;
        [SerializeField] private InterfaceReference<ISignal<float>> _b5;
        
        private protected override ISignal<float> GetSignal(int index) => index switch
        {
            0 => _b1.Value,
            1 => _b2.Value,
            2 => _b3.Value,
            3 => _b4.Value,
            4 => _b5.Value,
            _ => null
        };
    }
}