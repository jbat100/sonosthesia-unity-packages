using FMOD;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public abstract class FMODProcessor : MonoBehaviour
    {
        private ChannelGroup _currentChannelGroup;
        
        public bool IsSetup { get; private set; }
        
        protected abstract bool PerformTrySetup(ChannelGroup channelGroup);
        
        protected abstract void PerformCleanup(ChannelGroup channelGroup);
        
        public bool TrySetup(ChannelGroup channelGroup)
        {
            Cleanup();
            bool result = PerformTrySetup(channelGroup);
            if (result)
            {
                _currentChannelGroup = channelGroup;
            }
            else
            {
                UnityEngine.Debug.LogError($"{this} failed setup");
                _currentChannelGroup = default;
            }
            IsSetup = result;
            return result;
        }

        public void Cleanup()
        {
            if (!IsSetup)
            {
                return;
            }
            IsSetup = false;
            PerformCleanup(_currentChannelGroup);
        }

        public virtual void Process() { }
    }
}