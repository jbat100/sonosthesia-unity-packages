using System;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    [Serializable]
    public class FloatTrajectorySettings : TrajectorySettings<float>
    {
        [SerializeField] private bool _grid;
        [SerializeField] private float _gridSize;
        [SerializeField] private float _gridJump;
        [SerializeField] private FloatTrajectory.GridSnap _gridSnap;

        protected override void TriggerBounded(ValueTrajectory<float> trajectory)
        {
            if (trajectory is FloatTrajectory floatTrajectory && _grid)
            {
                floatTrajectory.TriggerGridBounded(Duration, _gridJump, _gridSize, Velocity, _gridSnap);
            }
            else
            {
                base.TriggerBounded(trajectory);
            }
        }
    }

    
    [CreateAssetMenu(fileName = "FloatTrajectory", menuName = "Sonosthesia/Trajectory/FloatTrajectory")]
    public class FloatTrajectoryConfiguration : TrajectoryConfiguration<float, FloatTrajectorySettings>
    {
        
    }
}