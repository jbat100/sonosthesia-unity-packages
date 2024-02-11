using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

namespace Sonosthesia.Pack
{

    public class WebSocketWire : Wire
    {
        [SerializeField] private string _address = "ws://127.0.0.1";
        
        private WebSocket websocket;

        protected virtual void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }
        
        protected override async UniTask<bool> PerformSend(byte[] bytes)
        {
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.Send(bytes);
                return true;
            }
            return false;
        }
        
        protected override void Setup()
        {
            websocket = new WebSocket(_address);

            websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
                State = WireState.Connected;
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
                State = WireState.Disconnected;
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

                Receive(bytes);
            };
        }
        
        protected override async UniTask Connect()
        {
            State = WireState.Connecting;
            await websocket.Connect();
        }
        
        protected override async UniTask Disconnect()
        {
            await websocket.Close();
        }
    }
}