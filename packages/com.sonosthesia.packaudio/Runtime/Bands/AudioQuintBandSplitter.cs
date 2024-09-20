using Sonosthesia.Pack;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.PackAudio
{
    public class AudioQuintBandSplitter : AudioBandSplitter<PackedAudioQuintBands>
    {
        [SerializeField] private Signal<float> _b1;
        [SerializeField] private Signal<float> _b2;
        [SerializeField] private Signal<float> _b3;
        [SerializeField] private Signal<float> _b4;
        [SerializeField] private Signal<float> _b5;
        
        private protected override Signal<float> GetSignal(int index) => index switch
        {
            0 => _b1,
            1 => _b2,
            2 => _b3,
            3 => _b4,
            4 => _b5,
            _ => null
        };
    }
}