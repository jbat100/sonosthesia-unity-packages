using UnityEngine;

namespace Sonosthesia.Flow
{
    public class RGBAColorFollower : ArpegiatorFollower<Color>
    {
        [SerializeField] private Vector4 _scale = Vector4.one; 
        
        public override Color Follow(Color original, Color current, Color arpegiated)
        {
            return arpegiated + (Color) Vector4.Scale(current - original, _scale);
        }
    }
}