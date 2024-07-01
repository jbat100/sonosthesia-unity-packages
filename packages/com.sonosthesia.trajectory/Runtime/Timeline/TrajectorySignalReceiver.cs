using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Trajectory
{
    public class TrajectorySignalReceiver : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private TrajectoryController _controller;
        
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is not TrajectorySignalEmitter signalEmitter)
            {
                return;
            }
            Debug.Log($"{this} received asset with payload key {signalEmitter.key}");
            _controller.Trigger(signalEmitter.key, signalEmitter.invert);
        }
    }
}