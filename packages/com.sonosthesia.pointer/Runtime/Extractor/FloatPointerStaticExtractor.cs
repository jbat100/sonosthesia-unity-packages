using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "FloatPointerStaticExtractor", menuName = "Sonosthesia/Pointer/FloatPointerStaticExtractor")]
    public class FloatPointerStaticExtractor : StaticExtractor<PointerEvent, float>
    {
        public override bool Extract(PointerEvent e, out float value)
        {
            throw new System.NotImplementedException();
        }
    }
}