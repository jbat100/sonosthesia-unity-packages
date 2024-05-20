using System.IO;
using System.Linq;
using MessagePack;

namespace Sonosthesia.Pack
{
    public static class PackedFileUtils
    {
        public static AudioAnalysis[] ReadAnalysisFile(string assetPath)
        {
            byte[] buffer = File.ReadAllBytes(assetPath);
            PackedAudioAnalysis[] samples = MessagePackSerializer.Deserialize<PackedAudioAnalysis[]>(buffer);
            return samples.Select(s => s.Unpack()).ToArray();
        }
    }
}