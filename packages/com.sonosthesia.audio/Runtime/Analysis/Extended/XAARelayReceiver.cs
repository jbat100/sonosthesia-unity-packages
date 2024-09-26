using UniRx;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Audio
{
    public class XAARelayReceiver : MonoBehaviour
    {
        [Header("Source")]
        
        [SerializeField] private XAARelay _relay;

        [Header("Output")] 
        
        [SerializeField] private Signal<ContinuousAnalysis> _continuous;
        [SerializeField] private Signal<PeakAnalysis> _peaks;

        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            if (_relay)
            {
                _subscriptions.Add(_relay.ContinuousAnalysisObservable.Subscribe(a =>
                {
                    if (_continuous)
                    {
                        _continuous.Broadcast(a);
                    }
                }));
                _subscriptions.Add(_relay.PeakAnalysisObservable.Subscribe(p =>
                {
                    if (_peaks)
                    {
                        _peaks.Broadcast(p);
                    }
                }));
            }
        }

        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
        }
    }
}