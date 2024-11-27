using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scheduler
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SchedulerTest))]
    public class SchedulerTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SchedulerTest test = (SchedulerTest)target;
            if(GUILayout.Button("Play"))
            {
                test.Play();
            }
            if(GUILayout.Button("Stop"))
            {
                test.Stop();
            }
        }
    }
#endif
    
    public class SchedulerTest : MonoBehaviour
    {
        [SerializeField] private AbstractScheduler _scheduler; 
        
        [SerializeField] private float _speed = 1f;
        
        [SerializeField] private float _chaos = 1f;
        
        private ISchedulerSession _session;
        private IDisposable _subscription;
        private float? _lastTime;

        protected void Update()
        {
            if (_session == null)
            {
                return;
            }
            
            _session.Speed = _speed;
            _session.Chaos = _chaos;
        }

        public void Play()
        {
            Stop();
            
            _session = _scheduler.CreateSession(_speed, _chaos);
            _subscription = _session.Stream.Subscribe(offset =>
            {
                if (_lastTime.HasValue)
                {
                    Debug.Log($"{this} scheduler fired {offset} ({Time.time - _lastTime.Value}) seconds since last)");
                }
                else
                {
                    Debug.Log($"{this} scheduler fired {offset}");
                }

                _lastTime = Time.time;
            });
        }

        public void Stop()
        {
            _subscription?.Dispose();
            _subscription = null;
            
            _session?.Dispose();
            _session = null;
        }
    }
}