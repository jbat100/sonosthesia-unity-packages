using UnityEngine;

namespace Sonosthesia.Scaffold
{
    public readonly struct BiSplineVertices
    {
        /// <summary>
        /// Orientation start
        /// </summary>
        public readonly Vector3 P0;
        
        /// <summary>
        /// Orientation end
        /// </summary>
        public readonly Vector3 P1;
        
        /// <summary>
        /// Reflected orientation start
        /// </summary>
        public readonly Vector3 P2;
        
        /// <summary>
        /// Reflected orientation end
        /// </summary>
        public readonly Vector3 P3;

        /// <summary>
        /// Four points expected to be coplanar
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public BiSplineVertices(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Plane Plane => new Plane(P0, P1, P3);

        public override string ToString()
        {
            return $"{nameof(BiSplineVertices)} {nameof(P0)} {P0} {nameof(P1)} {P1} {nameof(P2)} {P2} {nameof(P3)} {P3}";
        }
    }
}