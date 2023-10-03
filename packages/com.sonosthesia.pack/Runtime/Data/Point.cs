using MessagePack;
using UnityEngine;

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class Point
    {
        [Key("x")]
        public float X { get; set; }

        [Key("y")]
        public float Y { get; set; }

        [Key("z")]
        public float Z { get; set; }

        [Key("visibility")]
        public float Visibility { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} ({X}, {Y}, {Z}) Visibility : {Visibility}";
        }
    }

    public static class PointExtensions
    {
        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }

}