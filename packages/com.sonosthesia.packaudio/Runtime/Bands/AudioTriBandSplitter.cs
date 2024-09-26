using Sonosthesia.Pack;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.PackAudio
{
    public class AudioTriBandSplitter : AudioBandSplitter<PackedAudioTriBands>
    {
        [SerializeField] private Signal<float> _b1;
        [SerializeField] private Signal<float> _b2;
        [SerializeField] private Signal<float> _b3;

        private protected override Signal<float> GetSignal(int index) => index switch
        {
            0 => _b1,
            1 => _b2,
            2 => _b3,
            _ => null
        };
    }
}