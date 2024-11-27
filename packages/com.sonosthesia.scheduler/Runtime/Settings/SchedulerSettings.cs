using System;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
    [Serializable]
    public class SchedulerSettings
    {
        public enum SchedulerType
        {
            Custom,
            Random
        }

        [SerializeField] private SchedulerType _schedulerType = SchedulerType.Random;

        [SerializeField] private AbstractScheduler _scheduler;
        
        public ISchedulerSession MakeSession(float speed, float chaos)
        {
            return _schedulerType switch
            {
                SchedulerType.Custom => _scheduler.CreateSession(speed, chaos),
                SchedulerType.Random => new RandomSession(speed, chaos),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}