using UnityEngine;

namespace Sonosthesia.Application
{
    [CreateAssetMenu(fileName = "ApplicationConfiguration", menuName = "Sonosthesia/Application/ApplicationConfiguration")]
    public class ApplicationConfiguration : ScriptableObject
    {
        [SerializeField] private SceneSwitcherSettings _sceneSwitcherSettings;
        public SceneSwitcherSettings SceneSwitcherSettings => _sceneSwitcherSettings;
    }
}