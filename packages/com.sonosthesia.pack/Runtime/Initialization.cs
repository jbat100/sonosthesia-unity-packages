using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class Startup
    {
        static bool serializerRegistered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register(
                    Resolvers.GeneratedResolver.Instance,
                    StandardResolver.Instance
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
}