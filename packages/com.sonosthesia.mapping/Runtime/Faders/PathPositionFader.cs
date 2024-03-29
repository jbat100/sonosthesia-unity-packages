using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public class PathPositionFader : Fader<Vector3>
    {
        [SerializeField] private Path _path;
        
        [SerializeField] private bool _normlized = true;
        
        public override Vector3 Fade(float fade)
        {
            return _path.GetPosition(fade, _normlized);
        }
    }
}