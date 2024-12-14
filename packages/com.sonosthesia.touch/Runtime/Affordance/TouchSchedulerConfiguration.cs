using Sonosthesia.Scheduler;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchSchedulerConfiguration", menuName = "Sonosthesia/Touch/TouchSchedulerConfiguration")]
    public class TouchSchedulerConfiguration : ScriptableObject
    {
        [SerializeField] private SchedulerSettings _scheduler;
        public SchedulerSettings Scheduler => _scheduler;

        [SerializeField] private TouchEnvelopeSettings _speed;
        public TouchEnvelopeSettings Speed => _speed;
        
        [SerializeField] private TouchEnvelopeSettings _chaos;
        public TouchEnvelopeSettings Chaos => _chaos;
    }
}