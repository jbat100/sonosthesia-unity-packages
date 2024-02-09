using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    [CreateAssetMenu(fileName = "TriBandMapper", menuName = "Sonosthesia/Mappers/TriBandMapper")]
    public class TriBandMapper : BandMapper
    {
        [SerializeField] private ProcessorSettings _lows;
        [SerializeField] private ProcessorSettings _mids;
        [SerializeField] private ProcessorSettings _highs;
        
        public override IDisposable Map(MapperConnection<float> source, MapperConnection<float> target)
        {
            return new CompositeDisposable()
            {
                MapperUtils.Connect("Lows", source, target, _lows.Make()),
                MapperUtils.Connect("Mids", source, target, _mids.Make()),
                MapperUtils.Connect("Highs", source, target, _highs.Make())
            };
        }
    }
}