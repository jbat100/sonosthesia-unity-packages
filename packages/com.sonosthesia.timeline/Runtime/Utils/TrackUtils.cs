using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Timeline
{
    public static class TrackUtils
    {
        // GetGameObjectBinding is an internal method in Unity.Timeline
        public static GameObject GetGameObjectBinding(Object obj, PlayableDirector director)
        {
            if (director == null)
                return null;

            var binding = director.GetGenericBinding(obj);

            var gameObject = binding as GameObject;
            if (gameObject != null)
                return gameObject;

            var comp = binding as Component;
            if (comp != null)
                return comp.gameObject;

            return null;
        }
        
        public static Component GetComponentBinding(Object obj, PlayableDirector director)
        {
            if (director == null)
                return null;

            var binding = director.GetGenericBinding(obj);

            return binding as Component;
        }
    }
}