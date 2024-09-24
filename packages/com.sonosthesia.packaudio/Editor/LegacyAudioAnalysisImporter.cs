using System;
using System.Linq;
using Sonosthesia.Audio;
using Sonosthesia.Pack;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Sonosthesia.PackAudio.Editor
{
    [Obsolete("This is for backward compatibility with early versions of the sonosthesia-audio-pipeline")]
    [ScriptedImporter(1, "aad")]
    public class LegacyAudioAnalysisImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            ContinuousAnalysis[] samples = PackedFileUtils
                .ReadFile<PackedAudioAnalysis[]>(ctx.assetPath)
                .Select(a => a.Unpack()).ToArray();
            
            LegacyAudioAnalysisAsset asset = ScriptableObject.CreateInstance<LegacyAudioAnalysisAsset>();
            Debug.Log($"{this} {nameof(OnImportAsset)} imported {samples.Length} {nameof(ContinuousAnalysis)} samples");
            asset.samples = samples;
            asset.name = assetName;
            ctx.AddObjectToAsset(nameof(LegacyAudioAnalysisAsset), asset);
            ctx.SetMainObject(asset);
        }
    }
}