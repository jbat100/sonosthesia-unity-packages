using System;
using Sonosthesia.Noise;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    // Once you alter ComponentCount or Length, old array references may be invalid

    public class UnsafeNativeArraySummationHelper<T> : IDisposable where T : struct
    {
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
                }
            }
        }

        public NativeArray<T>[] terms;
        public NativeArray<T> sum;
        
        private const Allocator _allocator = Allocator.Persistent;
        private const NativeArrayOptions _options = NativeArrayOptions.UninitializedMemory;

        public void Check()
        {
            if (_dirty)
            {
                _dirty = false;
                InitializeArrays();
            }
        }
        
        private void InitializeArrays()
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
                for (int i = commonSize; i < old.Length; i++)
                {
                    old[i].Dispose();
                }
                for (int i = commonSize; i < _componentCount; i++)
                {
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
        public static JobHandle SumFloats(this UnsafeNativeArraySummationHelper<float> helper, JobHandle dependency)
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