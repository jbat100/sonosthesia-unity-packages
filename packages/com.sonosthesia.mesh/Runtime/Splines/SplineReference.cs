using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    [Serializable]
    public class SplineReference
    {
        [SerializeField] private SplineContainer _splineContainer;

        [SerializeField] private int _splineIndex;
        
        public Spline Spline
        {
            get
            {
                if (!_splineContainer)
                {
                    return null;
                }

                return _splineContainer.Splines.Count <= _splineIndex ? null : _splineContainer[_splineIndex];
            }
        }

    }
}