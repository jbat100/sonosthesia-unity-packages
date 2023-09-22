using UnityEngine;

namespace Sonosthesia.Utils
{
    public readonly struct Trans
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;

        public override string ToString()
        {
            return $"{nameof(Trans)} {nameof(Position)} {Position} {nameof(Rotation)} {Rotation} {nameof(Scale)} {Scale}";
        }

        public Trans(Vector3 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }
        
        public Trans(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Scale = Vector3.one;
        }
        
        public Trans(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Trans(Transform transfrom)
        {
            Position = transfrom.localPosition;
            Rotation = transfrom.localRotation;
            Scale = transfrom.localScale;
        }

        public void ApplyTo(Transform transform, bool applyScale = true, bool applyPosition = true, bool applyRotation = true)
        {
            if (applyPosition)
                transform.localPosition = Position;
            if (applyRotation)
                transform.localRotation = Rotation;
            if (applyScale)
                transform.localScale = Scale;
        }
    }
}


