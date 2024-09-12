using Sonosthesia.Pack;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.Audio.Editor
{
    [ScriptedImporter(1, "aadc")]
    public class AudioAnalysisContainerImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            AudioAnalysis[] samples = PackedFileUtils.ReadAnalysisFile(ctx.assetPath);
            AudioAnalysisAsset asset = ScriptableObject.CreateInstance<AudioAnalysisAsset>();
            asset.samples = samples;
            
            AudioAnalysisFileAsset fileAsset = ScriptableObject.CreateInstance<AudioAnalysisFileAsset>();
            fileAsset.analysis = asset;
            
            fileAsset.name = assetName;
            ctx.AddObjectToAsset("AudioAnalysisFileAsset", fileAsset);
            ctx.SetMainObject(fileAsset);

            asset.name = "Samples";
            ctx.AddObjectToAsset("Analysis", asset);
        }
    }
}