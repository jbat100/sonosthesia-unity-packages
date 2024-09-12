using UnityEngine;

namespace Sonosthesia.Audio
{
    [RequireComponent(typeof(AudioSpectrum))]
    public class AudioSpectrumLog : MonoBehaviour
    {
        [SerializeField] private LevelType _levelType;
        
        private AudioSpectrum _spectrum;

        protected void Awake()
        {
            _spectrum = GetComponent<AudioSpectrum>();
        }

        protected void Update()
        {
            Debug.Log($"{this} audio spectrum is ({string.Join(", ", _spectrum.GetLevels(_levelType))})");
        }
    }
}