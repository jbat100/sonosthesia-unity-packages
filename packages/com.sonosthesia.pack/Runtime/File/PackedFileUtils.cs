using System.IO;
using MessagePack;

namespace Sonosthesia.Pack
{
    public static class PackedFileUtils
    {
        public static T ReadFile<T>(string assetPath)
        {
            byte[] buffer = File.ReadAllBytes(assetPath);
            return ReadFile<T>(buffer);
        }
        
        public static T ReadFile<T>(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<T>(buffer);
        }
    }
}