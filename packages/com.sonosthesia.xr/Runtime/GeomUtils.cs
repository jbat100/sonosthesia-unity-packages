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

        public static bool UpperBind(Vector3 bind, Quaternion rotation, Axes axes, ref Vector3 target)
        {
            bool bound = false;

            if (axes.HasFlag(Axes.X))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.left, ref target);
            }
            if (axes.HasFlag(Axes.Y))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.down, ref target);
            }
            if (axes.HasFlag(Axes.Z))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.back, ref target);
            }

            return bound;
        }
        
        public static bool LowerBind(Vector3 bind, Quaternion rotation, Axes axes, ref Vector3 target)
        {
            bool bound = false;

            if (axes.HasFlag(Axes.X))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.right, ref target);
            }
            if (axes.HasFlag(Axes.Y))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.up, ref target);
            }
            if (axes.HasFlag(Axes.Z))
            {
                bound |= BindTargetToPlane(bind, rotation * Vector3.forward, ref target);
            }

            return bound;
        }
    }
}