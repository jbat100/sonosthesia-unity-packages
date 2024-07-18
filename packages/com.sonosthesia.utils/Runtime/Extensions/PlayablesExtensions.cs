using System;
using UnityEngine.Playables;

namespace Sonosthesia.Utils
{
    public static class PlayablesExtensions
    {
        public static void SafeSetTime(this PlayableDirector playableDirector, double time)
        {
            double safe = Math.Clamp(time, 0f, playableDirector.duration - 1e-3);
            playableDirector.time = safe;
        }
        
        public static void SafeJump(this PlayableDirector playableDirector, double size)
        {
            playableDirector.SafeSetTime(playableDirector.time + size);
        }
    }
}