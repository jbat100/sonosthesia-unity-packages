using UnityEngine;
using MessagePack;
using NativeWebSocket;

using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

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
        public string Type { get; set; }

        [Key("content")]
        public byte[] Content { get; set; }
    }

    [MessagePackObject]
    public class Pose
    {
        [Key("x")]
        public float X { get; set; }

        [Key("y")]
        public float Y { get; set; }

        [Key("z")]
        public float Z { get; set; }

        [Key("visibility")]
        public float Visibility { get; set; }
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
        
        // Start is called before the first frame update
        async void Start()
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
                Debug.Log("OnMessage!");
                Debug.Log(bytes);

                // getting the message as a string
                // var message = System.Text.Encoding.UTF8.GetString(bytes);
                //Debug.Log("OnMessage! " + message);

                Counting counting = MessagePackSerializer.Deserialize<Counting>(bytes);
                Debug.Log($"{counting}");
            };

            // Keep sending messages at every 0.3s
            InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

            // waiting for messages
            await websocket.Connect();
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

        private async void OnApplicationQuit()
        {
            await websocket.Close();
        }
        
    }
}