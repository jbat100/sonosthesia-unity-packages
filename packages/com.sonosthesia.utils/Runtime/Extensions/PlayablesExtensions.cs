using System;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Utils
{
    public static class PlayablesExtensions
    {
        public static void SafeSetTime(this PlayableDirector playableDirector, double time)
        {
            Debug.Log($"{nameof(SafeSetTime)} {time}");
            
            double safe = Math.Clamp(time, 0f, playableDirector.duration - 1e-3);

            bool wasPlaying = playableDirector.state == PlayState.Playing;
            
            
            playableDirector.time = safe;

            if (wasPlaying)
            {
                playableDirector.Pause();
                playableDirector.Resume();
            }
        }
        
        public static void SafeJump(this PlayableDirector playableDirector, double size)
        {
            playableDirector.SafeSetTime(playableDirector.time + size);
        }
    }
}