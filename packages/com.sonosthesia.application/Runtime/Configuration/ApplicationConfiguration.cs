using System;
using UnityEngine;

namespace Sonosthesia.Application
{
    [Serializable]
    public class ApplicationSettings
    {
        [SerializeField] private Vector3 _contentOffset;
        public Vector3 ContentOffset => _contentOffset;

        [SerializeField] private bool _testInteraction;
        public bool TestInteraction => _testInteraction;
    }
    
    [CreateAssetMenu(fileName = "ApplicationConfiguration", menuName = "Sonosthesia/Application/ApplicationConfiguration")]
    public class ApplicationConfiguration : ScriptableObject
    {
        [SerializeField] private ApplicationSettings _applicationSettings;
        public ApplicationSettings ApplicationSettings => _applicationSettings;

        [SerializeField] private SceneSwitcherSettings _sceneSwitcherSettings;
        public SceneSwitcherSettings SceneSwitcherSettings => _sceneSwitcherSettings;
    }
}