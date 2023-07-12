using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public readonly struct DynamicTransform
    {
        public readonly struct Data
        {
            public readonly Vector3 Position;
            public readonly Vector3 Rotation;

            public Data(Vector3 position, Vector3 rotation)
            {
                Position = position;
                Rotation = rotation;
            }

            public static Data Differential(Data previous, Data current, float deltaTime)
            {
                return (current - previous) / deltaTime;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator +(Data a, Data b) => new Data(a.Position + b.Position, a.Rotation + b.Rotation);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator -(Data a, Data b) => new Data(a.Position - b.Position, a.Rotation - b.Rotation);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator *(Data a, float d) => new Data(a.Position * d, a.Rotation * d);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator *(float d, Data a) => new Data(a.Position * d, a.Rotation * d);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator /(Data a, float d) => new Data(a.Position / d, a.Rotation / d);
        }

        public readonly Data Velocity;
        public readonly Data Acceleration;
        public readonly Data Jerk;

        public DynamicTransform(Data velocity, Data acceleration, Data jerk)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Jerk = jerk;
        }
    }
    
}