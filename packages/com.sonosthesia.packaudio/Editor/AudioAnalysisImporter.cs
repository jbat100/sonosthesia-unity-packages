using System.Linq;
using Sonosthesia.Audio;
using Sonosthesia.Pack;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.PackAudio.Editor
{
    [ScriptedImporter(1, "aad")]
    public class AudioAnalysisImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            AudioAnalysis[] samples = PackedFileUtils
                .ReadFile<PackedAudioAnalysis[]>(ctx.assetPath)
                .Select(a => a.Unpack()).ToArray();
            
            AudioAnalysisAsset asset = ScriptableObject.CreateInstance<AudioAnalysisAsset>();
            Debug.Log($"{this} {nameof(OnImportAsset)} imported {samples.Length} {nameof(AudioAnalysis)} samples");
            asset.samples = samples;
            asset.name = assetName;
            ctx.AddObjectToAsset("AudioAnalysisFileAsset", asset);
            ctx.SetMainObject(asset);
        }
    }
}