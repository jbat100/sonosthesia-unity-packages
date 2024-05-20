using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.Audio.Editor
{
    [ScriptedImporter(1, "aadf")]
    public class AudioAnalysisFloatImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            using FileStream fileStream = new FileStream(ctx.assetPath, FileMode.Open);
            using BinaryReader reader = new BinaryReader(fileStream);
            
            long count = fileStream.Length / 4; // 4 bytes for each float (32 bits)
            float[] data = new float[count];

            for (long i = 0; i < count; i++)
            {
                data[i] = reader.ReadSingle();
            }

            AudioAnalysisFloatAsset asset = ScriptableObject.CreateInstance<AudioAnalysisFloatAsset>();
            asset.data = data;
            asset.name = assetName;
            ctx.AddObjectToAsset("AudioAnalysisFileAsset", asset);
            ctx.SetMainObject(asset);
        }
    }
}