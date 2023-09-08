using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class ProcessingChainOperator<T> : SimpleOperator<T> where T : struct
    {
        [SerializeField] private List<DynamicProcessorFactory<T>> _operatorFactories;

        [SerializeField] private float _timeScale = 1f;

        private IDynamicProcessor<T> _operator;

        protected void Awake() => _operator = new ProcessorChain<T>(
            _operatorFactories.Select(f => f.Make()).ToArray()
            );

        protected override void OnEnable()
        {
            base.OnEnable();
            _operator.Reset();
        }

        protected override T Process(T input) => _operator.Process(input, Time.time * _timeScale);
    }
}