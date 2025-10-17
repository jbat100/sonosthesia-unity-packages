using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    public enum PointerRaycastSpace
    {
        World,
        Target
    }
    
    public static class PointerExtractionUtils
    {
        public static bool ExtractScreen(this PointerEvent e, 
            Axes axes, VectorFloatSelector selector, out float value)
        {
            value = e.Data.position.FilterAxes(axes).magnitude;
            return true;
        }
        
        public static bool ExtractRaycast(this PointerEvent e, PointerRaycastSpace space, 
            Axes axes, VectorFloatSelector selector, out float value)
        {
            Vector3 Project(Vector3 position) => space switch
            {
                PointerRaycastSpace.Target => e.Data.pointerEnter.transform.InverseTransformPoint(position),
                _ => position
            };
                
            Vector3 drag = Project(e.Data.position);   
            value = drag.FilterAxes(axes).magnitude;
            return true;
        }
    }
}