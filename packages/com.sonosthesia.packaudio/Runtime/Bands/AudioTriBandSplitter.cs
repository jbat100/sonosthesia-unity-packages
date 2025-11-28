using Sonosthesia.Pack;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.PackAudio
{
    public class AudioTriBandSplitter : AudioBandSplitter<PackedAudioTriBands>
    {
        [SerializeField] private InterfaceReference<ISignal<float>> _b1;
        [SerializeField] private InterfaceReference<ISignal<float>> _b2;
        [SerializeField] private InterfaceReference<ISignal<float>> _b3;

        private protected override ISignal<float> GetSignal(int index) => index switch
        {
            0 => _b1.Value,
            1 => _b2.Value,
            2 => _b3.Value,
            _ => null
        };
    }
}