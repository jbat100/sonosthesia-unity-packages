using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace Sonosthesia.Pack
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(WebSocketClient), true)]
    public class WebSocketClientEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WebSocketClient client = (WebSocketClient)target;
            if(GUILayout.Button("Reconnect"))
            {
                client.Reconnect().Forget();
            }
        }
    }
#endif

    public class WebSocketClient : MonoBehaviour
    {
        [SerializeField] private string _address;
        
        private WebSocket websocket;

        public async UniTask Reconnect()
        {
            await Disconnect();
            await Connect();
        }
        
        protected virtual void Start()
        {
            Setup();
        }

        protected virtual void OnMessage(byte[] bytes)
        {
            
        }

        protected virtual void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }
        
        private readonly SemaphoreSlim _semaphore = new (1, 1);

        public async UniTask<bool> Send(byte[] bytes)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (websocket.State == WebSocketState.Open)
                {
                    await websocket.Send(bytes);
                    return true;
                }

                return false;
            }
            finally
            {
                _semaphore.Release();
            }
            
        }
        
        public async UniTask<bool> Send(string text)
        {
            return await Send(Encoding.UTF8.GetBytes(text));
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

        private void Setup()
        {
            websocket = new WebSocket(_address);

            websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            websocket.OnMessage += (bytes) =>
            {
                //Debug.Log("OnMessage!");
                //Debug.Log(bytes);

                // getting the message as a string
                // var message = System.Text.Encoding.UTF8.GetString(bytes);
                //Debug.Log("OnMessage! " + message);

                //TypedEnvelope envelope = MessagePackSerializer.Deserialize<TypedEnvelope>(bytes);
                //Debug.Log($"Received {nameof(TypedEnvelope)} with type {envelope.Type}");
                //_envelopeSubject.OnNext(envelope);

                OnMessage(bytes);
            };

            // Keep sending messages at every 0.3s
            //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

            Connect().Forget();
        }
        
        private async UniTask Connect()
        {
            await websocket.Connect();
        }
        
        private async UniTask Disconnect()
        {
            await websocket.Close();
        }
    }
}