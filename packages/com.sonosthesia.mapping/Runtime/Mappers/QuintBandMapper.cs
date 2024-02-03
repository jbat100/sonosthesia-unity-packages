using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    [CreateAssetMenu(fileName = "QuintBandMapper", menuName = "Sonosthesia/Mappers/QuintBandMapper")]
    public class QuintBandMapper : BandMapper
    {
        [SerializeField] private ProcessorSettings _lows;
        [SerializeField] private ProcessorSettings _lowerMids;
        [SerializeField] private ProcessorSettings _mids;
        [SerializeField] private ProcessorSettings _higherMids;
        [SerializeField] private ProcessorSettings _highs;
        
        public override IDisposable Map(MapperConnection<float> source, MapperConnection<float> target)
        {
            return new CompositeDisposable()
            {
                MapperUtils.Connect("Lows", source, target, _lows.Make()),
                MapperUtils.Connect("LowerMids", source, target, _lowerMids.Make()),
                MapperUtils.Connect("Mids", source, target, _mids.Make()),
                MapperUtils.Connect("HigherMids", source, target, _higherMids.Make()),
                MapperUtils.Connect("Highs", source, target, _highs.Make())
            };
        }
    }
}