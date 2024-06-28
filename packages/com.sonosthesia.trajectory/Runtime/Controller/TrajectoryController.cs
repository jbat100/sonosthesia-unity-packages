using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public abstract class TrajectoryController : MonoBehaviour
    {
        public abstract void Trigger(string key);
    }
}