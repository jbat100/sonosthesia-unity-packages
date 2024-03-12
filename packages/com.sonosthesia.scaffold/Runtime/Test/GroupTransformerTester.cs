using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(GroupTransformerTester), true)]
    public class GroupTransformerTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GroupTransformerTester tester = (GroupTransformerTester)target;
            if(GUILayout.Button("Reload"))
            {
                tester.Reload();
            }
            if(GUILayout.Button("Apply"))
            {
                tester.Reload();
            }
        }
    }
#endif
    
    public abstract class GroupTransformerTester : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        
        [SerializeField] private GroupTransformerElement _prefab;

        [SerializeField] private List<GroupTransformer> _groupTransformers;

        private readonly List<GroupTransformerElement> _elements = new();

        private readonly CompositeDisposable _changeSubscriptions = new ();

        protected abstract IEnumerable<float> Offsets { get; }

        protected virtual void Awake()
        {
            if (!_root)
            {
                _root = transform;
            }
        }
        
        protected virtual void OnEnable()
        {
            _changeSubscriptions.Clear();
            foreach (GroupTransformer transformer in _groupTransformers)
            {
                _changeSubscriptions.Add(transformer.ChangeObservable.Subscribe(_ => Apply()));
            }
            Reload();
        }

        protected virtual void OnDisable()
        {
            _changeSubscriptions.Clear();
            Clear();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (EditorApplication.isPlaying && isActiveAndEnabled)
            {
                Reload();    
            }
        }
#endif

        private void Clear()
        {
            foreach (GroupTransformerElement element in _elements)
            {
                Destroy(element.Transform.gameObject);
            }
            _elements.Clear();
        }
        
        public void Reload()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return;
            }
#endif
            
            Clear();
            foreach (float offset in Offsets)
            {
                GroupTransformerElement element = Instantiate(_prefab, _root);
                element.Offset = offset;
                _elements.Add(element);
            }
            Apply();
        }

        public void Apply()
        {
            foreach (GroupTransformer transformer in _groupTransformers)
            {
                transformer.Apply(_elements);    
            }
        }
    }
}