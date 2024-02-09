using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class LayerExtensions
    {
        public static void SetLayerRecursively(this GameObject gameObject, int newLayer)
        {
            if (null == gameObject)
            {
                return;
            }
       
            gameObject.layer = newLayer;
       
            foreach (Transform child in gameObject.transform)
            {
                if (null == child)
                {
                    continue;
                }
                child.gameObject.SetLayerRecursively(newLayer);
            }
        }
    }
}