using Sonosthesia.Pack;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.Audio.Editor
{
    [ScriptedImporter(1, "aad")]
    public class AudioAnalysisImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            AudioAnalysis[] samples = PackedFileUtils.ReadAnalysisFile(ctx.assetPath);
            AudioAnalysisAsset asset = ScriptableObject.CreateInstance<AudioAnalysisAsset>();
            asset.samples = samples;
            asset.name = assetName;
            ctx.AddObjectToAsset("AudioAnalysisFileAsset", asset);
            ctx.SetMainObject(asset);
        }
    }
}