using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.XR
{
    public static class GeomUtils
    {
        public static bool BindTargetToPlane(Vector3 point, Vector3 normal, ref Vector3 target)
        {
            Plane plane = new Plane(normal, point);
            if (plane.GetDistanceToPoint(target) > 0f)
            {
                return false;
            }

            target = plane.ClosestPointOnPlane(target);
            return true;
        }

        //public static bool UpperBind(Vector3 point, Quaternion rotation, Axes axes, ref Vector3 target)
        //{
        //    if (axes.HasFlag(Axes.X))
        //    {
        //        
        //    }
        //}
    }
}