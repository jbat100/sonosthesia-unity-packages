using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia
{
    public class SyncUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _mainLabel;
        [SerializeField] private PlayableDirector _playableDirector;

        protected void Update()
        {
            _mainLabel.text = $"{_playableDirector.time:0.00} s";
        }
    }    
}


