using UnityEngine;
using Sonosthesia.Channel;
using Sonosthesia.Scheduler;

namespace Sonosthesia.Arpeggiator
{
    public class StaticArpeggiator<T> : Channel<T> where T : struct
    {
        [SerializeField] private LoopingScheduler _scheduler;
        
        [SerializeField] private Modulator<T> _modulator;
        
        [SerializeField] private ArpegiatorFollower<T> _follower;
    }
}