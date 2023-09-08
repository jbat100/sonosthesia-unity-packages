using System.Linq;

namespace Sonosthesia.Utils
{
    public class ProcessorChain<T> : IDynamicProcessor<T> where T : struct
    {
        private readonly IDynamicProcessor<T>[] _processors;

        public ProcessorChain(params IDynamicProcessor<T>[] processors)
        {
            _processors = processors.ToArray();
        }

        public T Process(T input, float time)
        {
            foreach (IDynamicProcessor<T> processor in _processors)
            {
                input = processor.Process(input, time);
            }

            return input;
        }

        public void Reset()
        {
            foreach (IDynamicProcessor<T> processor in _processors)
            {
                processor.Reset();
            }
        }
    }
}