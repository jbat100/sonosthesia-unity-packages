using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public class Vector3TrajectorySettings : TrajectorySettings<Vector3>
    {
        protected override Vector3 Invert(Vector3 value) => -value;
    }
    
    [CreateAssetMenu(fileName = "Vector3Trajectory", menuName = "Sonosthesia/Trajectory/Vector3Trajectory")]
    public class Vector3TrajectoryConfiguration : TrajectoryConfiguration<Vector3, Vector3TrajectorySettings>
    {
        
    }
}