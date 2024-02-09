using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(Wire), true)]
    public class WireEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Wire wire = (Wire)target;
            if(GUILayout.Button("Reconnect"))
            {
                wire.Reconnect().Forget();
            }
        }
    }
#endif
    
    // TODO : consider moving to separate package
    
    public enum WireState
    {
        Disconnected,
        Connecting,
        Connected
    }
    
    public abstract class Wire : MonoBehaviour
    {
        [SerializeField] private bool _autoConnect = true;
        
        private readonly BehaviorSubject<WireState> _stateSubject = new (WireState.Disconnected);
        public IObservable<WireState> StateObservable => _stateSubject.AsObservable();
        public WireState State
        {
            get => _stateSubject.Value;
            protected set => _stateSubject.OnNext(value);
        }

        private readonly Subject<byte[]> _messageSubject = new();
        public IObservable<byte[]> MessageObservable => _messageSubject.AsObservable();

        private readonly SemaphoreSlim _semaphore = new (1, 1);
        
        public async UniTask<bool> Send(string text)
        {
            return await Send(Encoding.UTF8.GetBytes(text));
        }
        
        public async UniTask<bool> Send(byte[] bytes)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await PerformSend(bytes);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        protected void Receive(byte[] message) => _messageSubject.OnNext(message);

        protected abstract UniTask<bool> PerformSend(byte[] bytes);

        protected abstract void Setup();
        
        protected abstract UniTask Connect();

        protected abstract UniTask Disconnect();
        
        public async UniTask Reconnect()
        {
            await Disconnect();
            await Connect();
        }
        
        protected virtual void Start()
        {
            Setup();
            if (_autoConnect)
            {
                Connect().Forget();
            }
        }
        
        protected virtual void OnDestroy()
        {
            Debug.Log($"{this} disconnect {nameof(OnDestroy)}");
            Disconnect().Forget();
        }
        
        protected virtual void OnApplicationQuit()
        {
            Debug.Log($"{this} disconnect {nameof(OnApplicationQuit)}");
            Disconnect().Forget();
        }
    }
}