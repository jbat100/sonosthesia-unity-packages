using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public class MultiConfiguration<TSettings> : ScriptableObject
    {
        [Serializable]
        private class KeyedSettings
        {
            [SerializeField] private string _key;
            public string Key => _key;

            [SerializeField] private TSettings _settings;
            public TSettings Settings => _settings;
        }

        [SerializeField] private List<KeyedSettings> _settings = new ();

        private readonly Dictionary<string, TSettings> _settingsMap = new ();

        public bool TryGet(string key, out TSettings settings)
        {
            return _settingsMap.TryGetValue(key, out settings);
        }
        
        private void CreateMap()
        {
            _settingsMap.Clear();
            foreach (KeyedSettings settings in _settings)
            {
                _settingsMap[settings.Key] = settings.Settings;
            }
        }

        protected virtual void OnEnable() => CreateMap();

        protected virtual void OnValidate() => CreateMap();
    }
}