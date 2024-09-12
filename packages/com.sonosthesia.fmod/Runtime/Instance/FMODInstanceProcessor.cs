using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using UniRx;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public class FMODInstanceProcessor : MonoBehaviour
    {
        [SerializeField] private FMODInstance _instance;

        [SerializeField] private List<FMODProcessor> _processors;
        
        private IDisposable _subscription;
        private EventInstance _currentInstance;

        public bool SetupDone { get; private set; }

        protected virtual void OnDestroy()
        {
            Cleanup();
        }
        
        protected virtual void OnEnable()
        {
            _subscription = _instance.InstanceObservable.Subscribe(instance =>
            {
                Cleanup();
                _currentInstance = instance;
                SetupDone = false;
            });
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            Cleanup();
        }
        
        protected virtual void Update()
        {
            if (_currentInstance.isValid() && !SetupDone)
            {
                SetupDone = TrySetup(_currentInstance);
            }
            
            if (!SetupDone)
            {
                return;
            }
            
            Process();
        }

        protected virtual bool TrySetup(EventInstance instance)
        {
            RESULT result = instance.getChannelGroup(out ChannelGroup channelGroup); 
            UnityEngine.Debug.LogWarning($"getChannelGroup {result}");
            if (result != RESULT.OK)
            {
                return false;
            }
            
            foreach (FMODProcessor processor in _processors)
            {
                if (!processor.TrySetup(channelGroup))
                {
                    UnityEngine.Debug.LogError($"{this} failed setup {processor}");
                    Cleanup();
                    return false;
                }
            }

            return true;
        }

        protected virtual void Cleanup()
        {
            foreach (FMODProcessor processor in _processors)
            {
                processor.Cleanup();
            }
        }
        
        /// <summary>
        /// Called on each Update if Processor is properly set up
        /// </summary>
        protected virtual void Process()
        {
            foreach (FMODProcessor processor in _processors)
            {
                processor.Process();
            }
        }
    }
}