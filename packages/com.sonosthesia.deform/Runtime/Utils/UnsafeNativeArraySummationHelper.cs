using System;
using System.Collections.Generic;
using Sonosthesia.Noise;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    // Once you alter ComponentCount or Length, old array references may be invalid

    public class UnsafeNativeArraySummationHelper<T> : IDisposable where T : struct
    {
        public UnsafeNativeArraySummationHelper(int poolSize = 0)
        {
            _poolSize = poolSize;
            if (_poolSize > 0)
            {
                _pool = new Stack<NativeArray<T>>();
            }
        }

        private readonly int _poolSize;
        private bool _dirty;
        
        private int _componentCount;
        public int ComponentCount
        {
            get => _componentCount;
            set
            {
                if (_componentCount != value)
                {
                    _componentCount = value;
                    _dirty = true;
                }
            }
        }
        
        private int _length;
        public int Length
        {
            get => _length;
            set
            {
                if (_length != value)
                {
                    _length = value;
                    _dirty = true;
                    ClearPool();
                }
            }
        }

        public NativeArray<T>[] terms;
        public NativeArray<T> sum;

        private readonly Stack<NativeArray<T>> _pool;
        
        private const Allocator _allocator = Allocator.Persistent;
        private const NativeArrayOptions _options = NativeArrayOptions.UninitializedMemory;

        public void Check()
        {
            if (_dirty)
            {
                _dirty = false;
                ReuseInitializeArrays();
            }
        }

        private void ClearPool()
        {
            if (_pool == null)
            {
                return;
            }
            foreach (NativeArray<T> pooled in _pool)
            {
                pooled.Dispose();
            }
            _pool.Clear();
        }

        // used to try to hunt down reuse bug
        private void InitializeArrays()
        {
            sum.Dispose();
            if (terms != null)
            {
                foreach (NativeArray<T> term in terms)
                {
                    term.Dispose();
                }    
            }

            terms = new NativeArray<T>[_componentCount];

            for (int i = 0; i < _componentCount; i++)
            {
                terms[i] = new NativeArray<T>(_length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            }
            
            sum = new NativeArray<T>(_length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }
        
        private void ReuseInitializeArrays()
        {
            if (terms == null)
            {
                terms = new NativeArray<T>[_componentCount];
                for (int i = 0; i < _componentCount; i++)
                {
                    terms[i].EnsureNativeArrayLength(_length, _allocator, _options);
                }
            }
            else 
            {
                NativeArray<T>[] old = terms;
                terms = new NativeArray<T>[_componentCount];
                int commonSize = math.min(old.Length, _componentCount);
                // reuse old arrays as much as possible
                for (int i = 0; i < commonSize; i++)
                {
                    terms[i] = old[i];
                    terms[i].EnsureNativeArrayLength(_length, _allocator, _options);
                }
                // in the case where previous number of terms was higher
                for (int i = commonSize; i < old.Length; i++)
                {
                    if (_pool != null && _pool.Count < _poolSize)
                    {
                        _pool.Push(old[i]);
                    }
                    else
                    {
                        old[i].Dispose();   
                    }
                }
                // in the case where previous number of terms was lower
                for (int i = commonSize; i < _componentCount; i++)
                {
                    if (_pool != null && _pool.TryPop(out NativeArray<T> reuse))
                    {
                        terms[i] = reuse;
                    }
                    terms[i].EnsureNativeArrayLength(_length, _allocator, _options);
                }
            }

            sum.EnsureNativeArrayLength(_length, _allocator, _options);
        }

        public void Dispose()
        {
            sum.Dispose();
            foreach (NativeArray<T> term in terms)
            {
                term.Dispose();
            }
        }
    }

    public static class UnsafeNativeArraySummationHelperExtensions
    {
        public static JobHandle FloatSum(this UnsafeNativeArraySummationHelper<float> helper, JobHandle dependency)
        {
            helper.sum.ClearArray();
            for (int i = 0; i < helper.ComponentCount; i++)
            {
                dependency = new FloatSumArrayJob
                {
                    source = helper.terms[i],
                    target = helper.sum
                }.ScheduleParallel(helper.Length, 100, dependency);
            }
            return dependency;
        }
        
        public static JobHandle Float4Sum(this UnsafeNativeArraySummationHelper<float4> helper, JobHandle dependency)
        {
            helper.sum.ClearArray();
            for (int i = 0; i < helper.ComponentCount; i++)
            {
                dependency = new Float4SumArrayJob
                {
                    source = helper.terms[i],
                    target = helper.sum
                }.ScheduleParallel(helper.Length, 100, dependency);
            }
            return dependency;
        }
        
        public static JobHandle Sum<T>(this UnsafeNativeArraySummationHelper<T> helper, JobHandle dependency) 
            where T : struct, ISummable<T>
        {
            helper.sum.ClearArray();
            for (int i = 0; i < helper.ComponentCount; i++)
            {
                dependency = new SumArrayJob<T>
                {
                    source = helper.terms[i],
                    target = helper.sum
                }.ScheduleParallel(helper.Length, 100, dependency);
            }

            return dependency;
        }
        
    }
}