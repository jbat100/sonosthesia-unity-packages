using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sonosthesia.Application
{
    public interface IPlayable
    {
        void Play();
    }
    
    // Abstract class potentially links to AudioSource, FMOD, Wwise or whatever other sound handling mechanism
    
    public class AudioController<T> : BaseAudioController where T : IPlayable
    {
        [Serializable]
        public class Setting
        {
            [SerializeField] private string _name;
            public string Name => _name;
            
            [SerializeField] private T _playable;
            public T Playable => _playable;
        }

        [SerializeField] private List<Setting> _settings;
        
        public override void Play(string eventName)
        {
            bool success = false;
            foreach (Setting setting in _settings.Where(s => s.Name == eventName))
            {
                Debug.Log($"{this} {nameof(Play)} {eventName}");
                setting.Playable?.Play();
                success = true;
            }

            if (!success)
            {
                Debug.LogWarning($"{this} {nameof(Play)} unsupported audio key : {eventName}");
            }
        }
    }
}