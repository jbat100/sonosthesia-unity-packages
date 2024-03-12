using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sonosthesia.Scaffold
{

    // Based on object handling sections of SplineInstantiate.cs to ensure that the instantiation is done 
    // and cleaned up correctly
    
    [ExecuteInEditMode]
    public abstract class GroupInstantiator<TEntry> : BaseGroupInstantiator where TEntry : MonoBehaviour, IIndexed
    {
        private const string k_InstancesRootName = "root-";
        private GameObject _instancesRoot;

        private Transform InstancesRoot
        {
            get
            {
                if (_instancesRoot == null)
                {
                    //Debug.Log($"{this} {nameof(TryClearCache)} creating root");
                    _instancesRoot = new GameObject(k_InstancesRootName+GetInstanceID());
                    _instancesRoot.hideFlags |= HideFlags.DontSave;
                    _instancesRoot.transform.parent = transform;
                    _instancesRoot.transform.localPosition = Vector3.zero;
                    _instancesRoot.transform.localRotation = Quaternion.identity;
                }
                return _instancesRoot.transform;
            }
        }

        private readonly List<TEntry> _instances = new ();
        internal List<TEntry> Instances => _instances;
        
        private bool _dirtyInstances = false;

        [SerializeField] private bool _autoRefresh = true;

        [SerializeField] private TEntry _prefab;
        
        private TEntry _previousPrefab;

        private IDisposable _refreshRequestSubscription;

        protected abstract int RequiredCount { get; }

        public override void Reload()
        {
            _dirtyInstances = true;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }

        protected virtual IObservable<Unit> RefreshRequestObservable => Observable.Empty<Unit>();

        private void RefreshRequestSubscription()
        {
            _refreshRequestSubscription?.Dispose();
            _refreshRequestSubscription = RefreshRequestObservable.Subscribe(_ => Reload());
        }

        protected virtual void Start()
        {
            RefreshRequestSubscription();
        }

        protected virtual void OnDestroy()
        {
            _refreshRequestSubscription?.Dispose();
            _dirtyInstances = true;
            TryClearCache();
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            Undo.undoRedoPerformed += UndoRedoPerformed;
#endif
            CheckChildrenValidity();
            UpdateInstances();
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            Undo.undoRedoPerformed -= UndoRedoPerformed;
#endif
            Clear();
        }

        protected virtual void UndoRedoPerformed()
        {
            _dirtyInstances = true;
        }

        protected virtual void OnValidate()
        {
            RefreshRequestSubscription();
            //Debug.Log($"{this} {nameof(OnValidate)}");
            _dirtyInstances = _autoRefresh;
        }

        private void CheckChildrenValidity()
        {
            // All the children have to be checked in case multiple GroupInstantiate components are used on the same GameObject.
            // We want to be able to have multiple components as it allows for example to instantiate grass and
            // trees with different parameters on the same object. 
            
            List<int> ids = GetComponents<GroupInstantiator<TEntry>>().Select(sInstantiate => sInstantiate.GetInstanceID()).ToList();
            int childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                var child = transform.GetChild(i).gameObject;
                if (child.name.StartsWith(k_InstancesRootName))
                {
                    var invalid = true;
                    foreach (int instanceID  in ids)
                    {
                        if (child.name.Equals(k_InstancesRootName + instanceID))
                        {
                            invalid = false;
                            break;
                        }
                    }
                    
                    if (invalid)
#if UNITY_EDITOR
                        DestroyImmediate(child);
#else
                        Destroy(child);
#endif
                }
            }
        }

        /// <summary>
        /// Clear all the created instances along the spline
        /// </summary>
        public void Clear()
        {
            SetDirty();
            TryClearCache();
            OnUpdatedInstances(_instances.AsReadOnly());
        }

        /// <summary>
        /// Set the created instances dirty to erase them next time instances will be generated
        /// (otherwise the next generation will reuse cached instances)
        /// </summary>
        public void SetDirty()
        {
            _dirtyInstances = true;
        }

        private void TryClearCache()
        {
            if (!_dirtyInstances)
            {
                for (int i = 0; i < _instances.Count; i++)
                {
                    if (_instances[i] == null)
                    {
                        _dirtyInstances = true;
                        break;
                    }
                }
            }

            if (_dirtyInstances)
            {
                //Debug.Log($"{this} {nameof(TryClearCache)} destroying instances and root");
                for (int i = _instances.Count - 1; i >= 0; --i)
                {
#if UNITY_EDITOR
                    DestroyImmediate(_instances[i]);
#else
                    Destroy(_instances[i]);
#endif
                }

#if UNITY_EDITOR
                DestroyImmediate(_instancesRoot);
#else
                Destroy(_instancesRoot);
#endif
                _instancesRoot = null;
                _instances.Clear();
                _dirtyInstances = false;
            }
        }


        protected virtual void Update()
        {
            if (!_dirtyInstances)
            {
                _dirtyInstances = (_previousPrefab == null || _previousPrefab != _prefab);    
            }

            if (_dirtyInstances)
            {
                UpdateInstances();
            }
        }
        
        /// <summary>
        /// Create and update all instances along the spline based on the list of available prefabs/objects.  
        /// </summary>
        public void UpdateInstances()
        {
            TryClearCache();
            EnsureCount(RequiredCount);
            _dirtyInstances = false;
            OnUpdatedInstances(_instances.AsReadOnly());
        }

        protected virtual void OnUpdatedInstances(IReadOnlyList<TEntry> instances)
        {
            
        }

        private void EnsureCount(int count)
        {
            if (_prefab == null)
            {
                return;
            }
            
            _previousPrefab = _prefab;
            
            if (count < _instances.Count)
            {
                //removing extra unnecessary instances
                for (int i = _instances.Count - 1; i >= count; i--)
                {
                    if (_instances[i] != null)
                    {
#if UNITY_EDITOR
                        DestroyImmediate(_instances[i]);
#else
                        Destroy(_Instances[i]);
#endif
                    }
                    _instances.RemoveAt(i);
                }                
            }

            if (count == _instances.Count)
            {
                return;
            }
            
#if UNITY_EDITOR
            var assetType = PrefabUtility.GetPrefabAssetType(_prefab);
            if (assetType == PrefabAssetType.MissingAsset)
            {
                return;
            }
#endif

            Debug.LogWarning($"{this} instantiating {count - _instances.Count} elements");
            
            for (int i = _instances.Count; i < count; i++)
            {
#if UNITY_EDITOR
                if (assetType != PrefabAssetType.NotAPrefab && !Application.isPlaying)
                    _instances.Add(PrefabUtility.InstantiatePrefab(_prefab, InstancesRoot) as TEntry);
                else
#endif
                    _instances.Add(Instantiate(_prefab, InstancesRoot));

                _instances[i].Index = i;

#if UNITY_EDITOR
                _instances[i].hideFlags |= HideFlags.HideAndDontSave;
                // Retrieve current static flags to pass them along on created instances
                var staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
                GameObjectUtility.SetStaticEditorFlags(_instances[i].gameObject, staticFlags);
#endif
            }
        }
    }
}