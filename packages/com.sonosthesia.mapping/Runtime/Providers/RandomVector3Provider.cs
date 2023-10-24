using System;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class RandomVector3Provider : ValueProvider<Vector3>
    {
        public enum Space
        {
            OnUnitSphere,
            InsideUnitSphere   
        }

        [SerializeField] private Space _space;

        [SerializeField] private float _scale;

        [SerializeField] private Vector3 _offset;
        
        public override Vector3 Value
        {
            get
            {
                Vector3 random = _space switch
                {
                    Space.OnUnitSphere => UnityEngine.Random.onUnitSphere,
                    Space.InsideUnitSphere => UnityEngine.Random.insideUnitSphere,
                    _ => throw new ArgumentOutOfRangeException()
                };
                return random * _scale + _offset;
            }
        }
    }
}