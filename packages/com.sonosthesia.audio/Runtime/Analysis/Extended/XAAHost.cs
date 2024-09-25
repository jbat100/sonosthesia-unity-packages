using UnityEngine;

namespace Sonosthesia.Audio
{
    public abstract class XAAHost : MonoBehaviour
    {
        [SerializeField] private bool _log;
        
        [SerializeField] private XAAConfiguration _configuration;

        public void Broadcast(ContinuousAnalysis analysis)
        {
            if (!_configuration)
            {
                PerformBroadcast(analysis);
                return;
            }

            analysis = _configuration.Process(analysis);
            PerformBroadcast(analysis);
        }

        public bool Broadcast(PeakAnalysis analysis)
        {
            if (!_configuration)
            {
                PerformBroadcast(analysis);
                return true;
            }
            
            if (!_configuration.Check(analysis))
            {
                if (_log)
                {
                    Debug.LogWarning($"{this} filtered {analysis}");
                }
                return false;
            }

            analysis = _configuration.Process(analysis);
            PerformBroadcast(analysis);
            return true;
        }

        protected abstract void PerformBroadcast(ContinuousAnalysis analysis);

        protected abstract void PerformBroadcast(PeakAnalysis analysis);
    }
}