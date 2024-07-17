using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Sonosthesia.Application
{
    // Note : note sure about this, needs testing and a cancellation token for WaitAsync
    
    public class OrderedSemaphore
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<UniTaskCompletionSource> _queue;

        public OrderedSemaphore(int initialCount)
        {
            _semaphore = new SemaphoreSlim(initialCount, initialCount);
            _queue = new ConcurrentQueue<UniTaskCompletionSource>();
        }

        public async UniTask WaitAsync()
        {
            var tcs = new UniTaskCompletionSource();
            _queue.Enqueue(tcs);
        
            // Wait for your turn to proceed
            await tcs.Task;
        
            // Once it's your turn, wait on the actual semaphore
            await _semaphore.WaitAsync();
        }

        public void Release()
        {
            _semaphore.Release();
        
            // Dequeue the next waiting task and set its result to allow it to proceed
            if (_queue.TryDequeue(out var next))
            {
                next.TrySetResult();
            }
        }
    }
}