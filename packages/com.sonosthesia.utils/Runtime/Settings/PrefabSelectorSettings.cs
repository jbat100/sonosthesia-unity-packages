using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sonosthesia.Utils
{
    public interface IPrefabSelectorSession<out T> where T : Object
    {
        T Next();
    }
    
    public enum PrefabSelectorType
    {
        Single,
        Multi
    }
    
    public enum PrefabMultiSelection
    {
        Sequential,
        Random
    }
    
    [Serializable]
    public class PrefabSelectorSettings<T> where T : Object
    {
        [SerializeField] private PrefabSelectorType _selectorType;

        [SerializeField] private T _prefab;

        [SerializeField] private PrefabMultiSelection _multiSelection;
        
        [SerializeField] private List<T> _prefabs;
        
        public IPrefabSelectorSession<T> MakeSession()
        {
            return _selectorType switch
            {
                PrefabSelectorType.Single => new SingleSession(_prefab),
                PrefabSelectorType.Multi => _multiSelection switch
                {
                    PrefabMultiSelection.Sequential => new SequentialSession(_prefabs.AsReadOnly()),
                    PrefabMultiSelection.Random => new RandomSession(_prefabs.AsReadOnly()),
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private class SingleSession : IPrefabSelectorSession<T>
        {
            private readonly T _prefab;
            
            public SingleSession(T prefab)
            {
                _prefab = prefab;
            }

            public T Next() => _prefab;
        }

        private class SequentialSession : IPrefabSelectorSession<T>
        {
            private readonly IReadOnlyList<T> _prefabs;
            private int _index;
            
            public SequentialSession(IReadOnlyList<T> prefabs)
            {
                _prefabs = prefabs;
            }

            public T Next()
            {
                if (_prefabs.Count == 0)
                {
                    return null;
                }
                _index = (_index + 1) % _prefabs.Count;
                return _prefabs[_index];
            }
        }

        private class RandomSession : IPrefabSelectorSession<T>
        {
            private readonly IReadOnlyList<T> _prefabs;
            
            public RandomSession(IReadOnlyList<T> prefabs)
            {
                _prefabs = prefabs;
            }
            
            public T Next()
            {
                if (_prefabs.Count == 0)
                {
                    return null;
                }
                return _prefabs[MathUtils.RandomInt(0, _prefabs.Count)];
            }
        }
    }
}