using System;
using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Generator
{
    /// <summary>
    /// Used mainly for investigation and debug purposes for now, runs a generator sequentially
    /// through a list of target signals 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TProcessor"></typeparam>
    public class SignalScanner<TValue, TProcessor> : MonoBehaviour where TValue: struct where TProcessor : IProcessor<TValue>
    {
        [Serializable]
        public class ScannerTarget
        {
            [SerializeField] private Signal<TValue> _signal;
            [SerializeField] private TProcessor _processor;

            public void Broadcast(TValue value)
            {
                _signal.Broadcast(_processor.Process(value));
            }
        }
        
        [SerializeField] private List<ScannerTarget> _targets;
        
        [SerializeField] private Generator<TValue> _generator;

        [SerializeField] private TValue _rest;

        [SerializeField] private float _timeScale = 1;
        
        [SerializeField] private float _duration = 1;

        [SerializeField] private float _sleep = 0;

        private float _time;
        
        protected virtual void OnEnable()
        {
            _time = 0;
        }

        protected virtual void Update()
        {
            float period = _duration + _sleep;
            int iteration = Mathf.FloorToInt(_time / period);
            int current = iteration % _targets.Count;
            float iterationTime = _time % period;
            bool sleep = iterationTime > _duration;
            
            for (int i = 0; i < _targets.Count; i++)
            {
                if (sleep || i != current)
                {
                    _targets[i].Broadcast(_rest);    
                }
                else
                {
                    _targets[i].Broadcast(_generator.Evaluate(iterationTime));
                }
            }
            
            _time += _timeScale * Time.deltaTime;
        }
    }
}