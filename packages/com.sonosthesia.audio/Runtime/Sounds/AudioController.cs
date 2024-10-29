using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sonosthesia.Audio
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
            public Setting(string name, T playable)
            {
                _name = name;
                _playable = playable;
            }
            
            [SerializeField] private string _name;
            public string Name => _name;
            
            [SerializeField] private T _playable;
            public T Playable => _playable;
        }

        [SerializeField] private List<Setting> _settings;

        public bool Has(string settingName)
        {
            return _settings.Any(s => s.Name == settingName);
        }
        
        public void Register(Setting setting)
        {
            _settings.Add(setting);
        }

        public void Unregister(string settingName)
        {
            _settings.RemoveAll(s => s.Name == settingName);
        }

        public void Clear()
        {
            _settings.Clear();
        }

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