using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Trajectory.Editor
{
    [CustomPropertyDrawer(typeof(TrajectorySettings<Vector3>))]
    public class Vector3TrajectorySettingsPropertyDrawer : TrajectorySettingsPropertyDrawer<Vector3>
    {
        
    }
}