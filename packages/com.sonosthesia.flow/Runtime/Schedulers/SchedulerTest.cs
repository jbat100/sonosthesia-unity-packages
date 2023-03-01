using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Flow
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
    
    [RequireComponent(typeof(Scheduler))]
    public class SchedulerTest : MonoBehaviour
    {
        private Scheduler _scheduler;
        private Scheduler.ISession _session;
        private IDisposable _subscription;
        private float? _lastTime;

        protected void Awake()
        {
            _scheduler = GetComponent<Scheduler>();
        }

        public void Play()
        {
            _session?.Dispose();
            _session = _scheduler.Session();
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
            _session?.Dispose();
        }
    }
}