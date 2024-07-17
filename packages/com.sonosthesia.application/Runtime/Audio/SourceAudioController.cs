using System;
using UnityEngine;

namespace Sonosthesia.Application
{
    [Serializable]
    public class SourcePlayable : IPlayable
    {
        [SerializeField] private AudioSource _source;

        [SerializeField] private AudioClip _clip;

        public void Play()
        {
            if (_source && _clip)
            {
                _source.PlayOneShot(_clip);
            }
            else if (_source)
            {
                _source.Play();
            }
        }
    }
    
    public class SourceAudioController : AudioController<SourcePlayable>
    {
        
    }
}