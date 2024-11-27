using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Application
{
    [CreateAssetMenu(fileName = "CompositionConfiguration", menuName = "Sonosthesia/Application/CompositionConfiguration")]
    public class CompositionConfiguration : ObservableScriptableObject
    {
        [SerializeField] [TextArea] private string _description;
        public string Description => _description;

        [SerializeField] private string _scenePath;
        public string ScenePath => _scenePath;

        [SerializeField] private Sprite _image;
        public Sprite Image => _image;
    }
}