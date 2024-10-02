using System.Runtime.CompilerServices;
using UnityEngine;

namespace Sonosthesia.Dynamic
{
    public readonly struct TransformDynamics
    {
        public enum Domain
        {
            Position,
            Rotation
        }

        public enum Order
        {
            None = 0,
            Velocity = 1,
            Acceleration = 2,
            Jerk = 3
        }
        
        public readonly struct Data
        {
            public readonly Vector3 Position;
            public readonly Vector3 Rotation;

            public Data(Vector3 position, Vector3 rotation)
            {
                Position = position;
                Rotation = rotation;
            }

            public Vector3 Select(Domain domain)
            {
                return domain switch
                {
                    Domain.Position => Position,
                    Domain.Rotation => Rotation,
                    _ => default
                };
            }
            
            public override string ToString()
            {
                return $"({nameof(Position)} : {Position}, {nameof(Rotation)} : {Rotation})";
            }

            public static Data Differential(Data previous, Data current, float deltaTime)
            {
                Data result = (current - previous) / deltaTime;
                if (result.Position.magnitude < 1e-4)
                {
                    //Debug.LogWarning($"{nameof(TransformDynamics)} very small differential");
                }

                return result;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator +(Data a, Data b) => new Data(a.Position + b.Position, a.Rotation + b.Rotation);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator -(Data a, Data b) => new Data(a.Position - b.Position, a.Rotation - b.Rotation);
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator *(Data a, float d) => new (a.Position * d, a.Rotation * d);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator *(float d, Data a) => new (a.Position * d, a.Rotation * d);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Data operator /(Data a, float d) => new (a.Position / d, a.Rotation / d);
        }

        public readonly Data Velocity;
        public readonly Data Acceleration;
        public readonly Data Jerk;

        public TransformDynamics(Data velocity, Data acceleration, Data jerk)
        {
            Velocity = velocity;
            Acceleration = acceleration;
            Jerk = jerk;
        }

        public Data Select(Order order)
        {
            return order switch
            {
                Order.Velocity => Velocity,
                Order.Acceleration => Acceleration,
                Order.Jerk => Jerk,
                _ => default
            };
        }

        public Vector3 Select(Order order, Domain domain)
        {
            return Select(order).Select(domain);
        }
        
        public override string ToString()
        {
            return $"{nameof(TransformDynamics)} {nameof(Velocity)} {Velocity} {nameof(Acceleration)} {Acceleration} {nameof(Jerk)} {Jerk}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TransformDynamics operator +(TransformDynamics a, TransformDynamics b)
        {
            return new TransformDynamics(a.Velocity + b.Velocity, a.Acceleration + b.Acceleration, a.Jerk + b.Jerk);   
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TransformDynamics operator -(TransformDynamics a, TransformDynamics b)
        {
            return new TransformDynamics(a.Velocity - b.Velocity, a.Acceleration - b.Acceleration, a.Jerk - b.Jerk);
        }
    }
    
}