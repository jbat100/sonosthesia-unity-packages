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
        
        protected override float Invert(float value) => -value;

        protected override void TriggerBounded(ValueTrajectory<float> trajectory, bool invert)
        {
            if (trajectory is FloatTrajectory floatTrajectory && _grid)
            {
                floatTrajectory.TriggerGridBounded(Duration, _gridJump, _gridSize, Process(Velocity, invert), _gridSnap);
            }
            else
            {
                base.TriggerBounded(trajectory, invert);
            }
        }
    }

    [CreateAssetMenu(fileName = "FloatTrajectory", menuName = "Sonosthesia/Trajectory/FloatTrajectory")]
    public class FloatTrajectoryConfiguration : TrajectoryConfiguration<float, FloatTrajectorySettings>
    {
        
    }
}