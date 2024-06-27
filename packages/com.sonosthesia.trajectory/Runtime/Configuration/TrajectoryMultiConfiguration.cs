using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Trajectory
{
    public class TrajectoryMultiConfiguration<TValue, TSettings> : ScriptableObject 
        where TValue : struct where TSettings : TrajectorySettings<TValue>
    {
        private class KeyedSettings
        {
            [SerializeField] private string _key;
            public string Key => _key;

            [SerializeField] private TSettings _settings;
            public TSettings Settings => _settings;
        }

        [SerializeField] private List<KeyedSettings> _settings;

        private Dictionary<string, TSettings> _settingsMap = new ();

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