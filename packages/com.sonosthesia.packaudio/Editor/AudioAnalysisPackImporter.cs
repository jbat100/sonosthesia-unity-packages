using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.PackAudio.Editor
{
    [ScriptedImporter(1, "aadb")]
    public class AudioAnalysisPackImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            AudioAnalysisPackAsset asset = ScriptableObject.CreateInstance<AudioAnalysisPackAsset>();
            asset.data = File.ReadAllBytes(ctx.assetPath);
            asset.name = assetName;
            ctx.AddObjectToAsset("AudioAnalysisFileAsset", asset);
            ctx.SetMainObject(asset);
        }
    }
}