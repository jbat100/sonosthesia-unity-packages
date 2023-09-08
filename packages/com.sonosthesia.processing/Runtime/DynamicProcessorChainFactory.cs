using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sonosthesia.Processing
{
    public class DynamicProcessorChainFactory<T> : DynamicProcessorFactory<T> where T : struct
    {
        [SerializeField] private List<DynamicProcessorFactory<T>> _chain;

        public override IDynamicProcessor<T> Make()
        {
            if (_chain.Contains(this))
            {
                throw new Exception("chain should not reference itself");
            }
            
            return new ProcessorChain<T>(_chain.Select(factory => factory.Make()).ToArray());
        }
    }
}