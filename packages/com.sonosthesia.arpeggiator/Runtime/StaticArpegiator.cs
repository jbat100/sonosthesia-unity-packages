using UnityEngine;

namespace Sonosthesia.Flow
{
    public class StaticArpegiator<T> : Channel<T> where T : struct
    {
        [SerializeField] private Scheduler _scheduler;
        
        [SerializeField] private Modulator<T> _modulator;
        
        [SerializeField] private ArpegiatorFollower<T> _follower;
    }
}