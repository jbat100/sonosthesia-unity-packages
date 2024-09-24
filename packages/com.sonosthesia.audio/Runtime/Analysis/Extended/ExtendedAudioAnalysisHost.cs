using UnityEngine;

namespace Sonosthesia.Audio
{
    public abstract class ExtendedAudioAnalysisHost : MonoBehaviour
    {
        [SerializeField] private ExtendedAudioAnalysisConfiguration _configuration;

        public void Broadcast(ContinuousAnalysis analysis)
        {
            if (!_configuration)
            {
                PerformBroadcast(analysis);
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