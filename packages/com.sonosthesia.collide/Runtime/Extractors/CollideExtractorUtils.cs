using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Collide
{
    public static class CollideExtractorUtils
    {
        public static bool ExtractContactPoint(this CollideEvent e, ExtractionSpace space, out Vector3 value)
        {
            value = default;
            return e.Collision.AveragePoint(out Vector3 point, c => c.point) 
                   && e.WorldToExtractionSpace(space, point, out value);
        }
        
        public static bool ExtractContactNormal(this CollideEvent e, ExtractionSpace space, out Vector3 value)
        {
            value = default;
            return e.Collision.AveragePoint(out Vector3 point, c => c.normal) 
                   && e.WorldToExtractionSpace(space, point, out value);
        }
        
        private static bool AveragePoint(this Collision collision, out Vector3 average, Func<ContactPoint, Vector3> extractor)
        {
            int count = collision.contactCount;

            if (count == 0)
            {
                average = Vector3.zero;
                return false;
            }

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                sum += extractor(collision.GetContact(i));
            }

            average = sum / count;
            return true;
        }
    }
}