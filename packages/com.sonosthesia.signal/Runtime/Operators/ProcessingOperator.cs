using Sonosthesia.Processing;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class ProcessingOperator<T> : SimpleOperator<T> where T : struct
    {
        [SerializeField] private DynamicProcessorFactory<T> _operatorFactory;

        [SerializeField] private float _timeScale = 1f;

        private IDynamicProcessor<T> _operator;

        protected void Awake() => _operator = _operatorFactory.Make();

        protected override void OnEnable()
        {
            base.OnEnable();
            _operator.Reset();
        }

        protected override T Process(T input) => _operator.Process(input, Time.time * _timeScale);
    }
}