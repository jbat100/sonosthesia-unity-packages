using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MessagePack;
using NativeWebSocket;

using MessagePack;
using MessagePack.Resolvers;
using UniRx;
using UnityEngine;
using UniRx;

public class Startup
{
    static bool serializerRegistered = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (!serializerRegistered)
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }
    }

#if UNITY_EDITOR


    [UnityEditor.InitializeOnLoadMethod]
    static void EditorInitialize()
    {
        Initialize();
    }

#endif
}

namespace Sonosthesia.Pack
{
    [MessagePackObject]
    public class Envelope
    {
        [Key("type")]
        public int Type { get; set; }

        [Key("content")]
        public byte[] Content { get; set; }
    }

    [MessagePackObject]
    public class Point
    {
        [Key("x")]
        public float X { get; set; }

        [Key("y")]
        public float Y { get; set; }

        [Key("z")]
        public float Z { get; set; }

        [Key("visibility")]
        public float Visibility { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} ({X}, {Y}, {Z}) Visibility : {Visibility}";
        }
    }

    public static class PointExtensions
    {
        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }
    }

    [MessagePackObject]
    public class Counting
    {
        [Key("counter")]
        public int Counter { get; set; }
        
        [Key("from")]
        public string From { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} {nameof(Counter)} {Counter} {nameof(From)} {From}";
        }
    }

    public class PackReceiver : MonoBehaviour
    {
        [SerializeField] private string _address;
        
        private WebSocket websocket;

        private readonly Subject<Envelope> _envelopeSubject = new();

        private readonly Dictionary<Type, int> _types = new Dictionary<Type, int>
        {
            {typeof(Counting), 1},
            {typeof(Point), 2},
            {typeof(MediapipePose), 1000}
        };
        
        public IObservable<T> PublishContent<T>()
        {
            if (!_types.TryGetValue(typeof(T), out int type))
            {
                throw new Exception("unsupported type");
            }

            return _envelopeSubject.Where(e => e.Type == type)
                .Select(envelope =>
                {
                    try
                    {
                        return MessagePackSerializer.Deserialize<T>(envelope.Content);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        throw;
                    }
                })
                .AsObservable();
        }
        
        // Start is called before the first frame update
        protected virtual void Start()
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

                Envelope envelope = MessagePackSerializer.Deserialize<Envelope>(bytes);
                Debug.Log($"Received {nameof(Envelope)} with type {envelope.Type}");
                _envelopeSubject.OnNext(envelope);
            };

            // Keep sending messages at every 0.3s
            //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

            Connect().Forget();
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }

        async void SendWebSocketMessage()
        {
            if (websocket.State == WebSocketState.Open)
            {
                // Sending bytes
                await websocket.Send(new byte[] { 10, 20, 30 });

                // Sending plain text
                await websocket.SendText("plain text message");
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