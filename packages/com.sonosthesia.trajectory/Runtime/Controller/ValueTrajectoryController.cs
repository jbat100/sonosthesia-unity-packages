using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public class ValueTrajectoryController<TValue, TSettings> : TrajectoryController 
        where TValue : struct where TSettings : TrajectorySettings<TValue>
    {
        [SerializeField] private List<TrajectoryMultiConfiguration<TValue, TSettings>.Controller> _controllers;
        
        public override void Trigger(string key)
        {
            foreach (TrajectoryMultiConfiguration<TValue, TSettings>.Controller controller in _controllers)
            {
                controller.Trigger(key);
            }
        }
    }
}