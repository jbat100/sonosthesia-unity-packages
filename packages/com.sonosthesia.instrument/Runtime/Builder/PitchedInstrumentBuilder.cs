using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentBuilder : MonoBehaviour
    {
        [SerializeField] private List<GroupTransformer> _transformers;

        [SerializeField] private Transform _prefab;

        [SerializeField] private int _startNote;
        
        [SerializeField] private int _endNote;

        [SerializeField] private int _rootNote;

        [SerializeField] private ScaleDescriptor _scaleDescriptor;
        
        
    }
}