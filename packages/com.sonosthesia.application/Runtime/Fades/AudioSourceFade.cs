using System;
using UnityEngine;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceFade : AbstractFade
    {
        private AudioSource _audioSource;

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        protected override void Apply(float fade)
        {
            _audioSource.volume = 1f - fade;
        }
    }
}