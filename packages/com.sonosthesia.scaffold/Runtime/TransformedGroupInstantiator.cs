using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    [ExecuteAlways]
    public abstract class TransformedGroupInstantiator<TEntry> : GroupInstantiator<TEntry> 
        where TEntry : MonoBehaviour, IGroupTransformerElement
    {
        [SerializeField] private List<GroupTransformer> _groupTransformers;

        private readonly CompositeDisposable _changeSubscriptions = new ();

        protected override int RequiredCount => Offsets.Count;

        protected abstract IReadOnlyList<float> ComputeOffsets();
        
        private IReadOnlyList<float> _offsets;
        private IReadOnlyList<float> Offsets
        {
            get
            {
                _offsets ??= ComputeOffsets();
                return _offsets;
            }
        }

        private bool _dirtyTransforms;
        
        protected virtual void Awake()
        {
            _changeSubscriptions.Clear();
            foreach (GroupTransformer transformer in _groupTransformers)
            {
                _changeSubscriptions.Add(transformer.ChangeObservable.Subscribe(_ => ApplyTransformers()));
            }
        }

        protected override void OnDestroy()
        {
            _changeSubscriptions.Clear();
            base.OnDestroy();
        }

        protected override void OnValidate()
        {
            _offsets = null;
            _dirtyTransforms = true;
            base.OnValidate();
        }

        protected override void Update()
        {
            base.Update();
            if (_dirtyTransforms)
            {
                ApplyTransformers();
            }
        }

        protected override void OnUpdatedInstances(IReadOnlyList<TEntry> instances)
        {
            base.OnUpdatedInstances(instances);
            IReadOnlyList<float> offsets = Offsets;
            int count = Mathf.Min(instances.Count, offsets.Count);
            for (int i = 0; i < count; i++)
            {
                instances[i].Offset = offsets[i];
            }
            ApplyTransformers();
        }

        public void ApplyTransformers()
        {
            Debug.Log($"{this} {nameof(ApplyTransformers)} ({_groupTransformers.Count})");
            foreach (GroupTransformer transformer in _groupTransformers)
            {
                transformer.Apply(Instances);    
            }

            _dirtyTransforms = false;
        }
    }
}