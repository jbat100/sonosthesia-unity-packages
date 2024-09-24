using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Audio
{
    [CreateAssetMenu(fileName = "ExtendedAudioAnalysisRelay", menuName = "Sonosthesia/Relays/ExtendedAudioAnalysisRelay")]
    public class ExtendedAudioAnalysisRelay : ScriptableObject
    {
        private readonly BehaviorSubject<ContinuousAnalysis> _analysisSubject = new(default);
        private readonly Subject<PeakAnalysis> _peakSubject = new();

        public IObservable<ContinuousAnalysis> ContinuousAnalysisObservable => _analysisSubject.AsObservable();
        public IObservable<PeakAnalysis> PeakAnalysisObservable => _peakSubject.AsObservable();

        public void Broadcast(ContinuousAnalysis analysis) => _analysisSubject.OnNext(analysis);
        public void Broadcast(PeakAnalysis analysis) => _peakSubject.OnNext(analysis);

        protected void OnDestroy()
        {
            _analysisSubject.OnCompleted();
            _peakSubject.OnCompleted();
            _analysisSubject.Dispose();
            _peakSubject.Dispose();
        }
    }
}