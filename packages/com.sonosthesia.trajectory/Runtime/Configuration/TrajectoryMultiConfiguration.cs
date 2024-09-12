using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public class TrajectoryMultiConfiguration<TValue, TSettings> : MultiConfiguration<TSettings> 
        where TValue : struct where TSettings : TrajectorySettings<TValue>
    {
        [Serializable]
        public class Controller
        {
            [SerializeField] private TrajectoryMultiConfiguration<TValue, TSettings> _configuration;
            [SerializeField] private ValueTrajectory<TValue> _trajectory;

            public void Trigger(string key, bool invert)
            {
                _configuration.Trigger(key, _trajectory, invert);
            }
        }
        
        public void Trigger(string key, ValueTrajectory<TValue> trajectory, bool invert)
        {
            if (TryGet(key, out TSettings settings))
            {
                settings.Trigger(trajectory, invert);
            }
        }
    }
}