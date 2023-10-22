using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [Flags]
    public enum Axes
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2
    }

    public static class AxesExtensions
    {
        public static Vector3 SetAxes(this Vector3 input, Vector3 set, Axes axes)
        {
            return new Vector3(
                axes.HasFlag(Axes.X) ? set.x : input.x,
                axes.HasFlag(Axes.Y) ? set.y : input.y,
                axes.HasFlag(Axes.Z) ? set.z : input.z
            );
        }

        public static Vector3 FilterAxes(this Vector3 input, Axes axes)
        {
            return SetAxes(input, Vector3.zero, ~axes);
        }
    }
}