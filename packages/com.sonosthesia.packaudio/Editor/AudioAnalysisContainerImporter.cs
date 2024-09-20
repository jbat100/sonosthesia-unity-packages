using System.Linq;
using Sonosthesia.Audio;
using Sonosthesia.Pack;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.PackAudio.Editor
{
    [ScriptedImporter(1, "aadc")]
    public class AudioAnalysisContainerImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            
            AudioAnalysis[] samples = PackedFileUtils.ReadFile<PackedAudioAnalysis[]>(ctx.assetPath).Select(a => a.Unpack()).ToArray();
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