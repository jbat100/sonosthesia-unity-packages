using System;
using System.IO;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;
using Sonosthesia.Audio;
using Sonosthesia.Pack;

namespace Sonosthesia.PackAudio.Editor
{
    [ScriptedImporter(1, "xaa")]
    public class PackedXAAImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            using FileStream fs = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new BinaryReader(fs);

            // first three int32 in the binary stream are for version and other format specifiers
            
            int version = reader.ReadInt32();
            reader.ReadInt32(); // reserved field 1
            reader.ReadInt32(); // reserved field 2

            byte[] packedData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            
            Debug.Log($"{this} {nameof(OnImportAsset)} read {packedData.Length} packed data bytes");

            XAAAsset asset = ScriptableObject.CreateInstance<XAAAsset>();

            if (version == 2)
            {
                PackedXAA analysis = PackedFileUtils.ReadBytes<PackedXAA>(packedData);
                if (analysis.Continuous != null)
                {
                    asset.Continuous = analysis.Continuous.Select(c => c.Unpack()).ToArray();   
                }
                else
                {
                    Debug.LogWarning($"{this} failed to unpack continuous analysis");
                }
                if (analysis.Peaks != null)
                {
                    asset.Peaks = analysis.Peaks.Select(p => p.Unpack()).ToArray();    
                }
                else
                {
                    Debug.LogWarning($"{this} failed to unpack peak analysis");
                }
                if (analysis.Info != null)
                {
                    asset.Info = analysis.Info.Unpack();
                }
                else
                {
                    Debug.LogWarning($"{this} failed to unpack analysis info");
                }
            }
            else
            {
                Debug.LogError($"{this} {nameof(OnImportAsset)} unsupported version {version}");
                asset.Continuous = Array.Empty<ContinuousAnalysis>();
                asset.Peaks = Array.Empty<PeakAnalysis>();
            }
            
            Debug.Log($"{this} {nameof(OnImportAsset)} imported " +
                      $"{asset.Continuous?.Length ?? 0} continuous analysis points and " +
                      $"{asset.Peaks?.Length ?? 0} peaks");
            
            asset.name = assetName;
            ctx.AddObjectToAsset(nameof(XAAAsset), asset);
            ctx.SetMainObject(asset);
        }
    }
}
