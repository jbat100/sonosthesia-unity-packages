using System.Text;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public static class CollisionExtensions
    {
        public static string ToDetailedString(this Collision collision)
        {
            StringBuilder builder = new StringBuilder($"Collision with {collision.contactCount} contact points :");
            foreach (ContactPoint point in collision.contacts)
            {
                builder.Append($"(point : {point.point}, separation : {point.separation}, normal : {point.normal})");
            }
            return builder.ToString();
        }
    }
}