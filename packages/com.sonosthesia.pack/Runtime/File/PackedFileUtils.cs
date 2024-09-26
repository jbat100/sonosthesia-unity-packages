using System.Buffers;
using System.IO;
using System.Threading;
using MessagePack;

namespace Sonosthesia.Pack
{
    public static class PackedFileUtils
    {
        public static T ReadFile<T>(string assetPath)
        {
            byte[] buffer = File.ReadAllBytes(assetPath);
            return ReadBytes<T>(buffer);
        }
        
        public static T ReadBytes<T>(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<T>(buffer);
        }
        
        public static T ReadFile<T>(ReadOnlySequence<byte> sequence)
        {
            return MessagePackSerializer.Deserialize<T>(sequence);
        }
    }
}