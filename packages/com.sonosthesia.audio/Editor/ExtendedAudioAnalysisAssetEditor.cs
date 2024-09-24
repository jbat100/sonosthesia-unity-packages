using UnityEditor;
using UnityEngine.UIElements;

namespace Sonosthesia.Audio.Editor
{
    [CustomEditor(typeof(ExtendedAudioAnalysisAsset))]
    public class ExtendedAudioAnalysisAssetEditor : UnityEditor.Editor
    {
        private VisualElement rootElement;

        public override VisualElement CreateInspectorGUI()
        {
            ExtendedAudioAnalysisAsset asset = (ExtendedAudioAnalysisAsset)target;
            rootElement = new VisualElement();

            if (asset.Info == null)
            {
                rootElement.Add(CreateMultilineLabel($"Info is unavailable"));
                return rootElement;
            }

            rootElement.Add(CreateMultilineLabel($"Duration :\n {asset.Info.Duration}"));
            rootElement.Add(CreateMultilineLabel($"Main :\n {asset.Info.Main}"));
            rootElement.Add(CreateMultilineLabel($"Lows :\n {asset.Info.Lows}"));
            rootElement.Add(CreateMultilineLabel($"Mids :\n {asset.Info.Mids}"));
            rootElement.Add(CreateMultilineLabel($"Highs :\n {asset.Info.Highs}"));
            rootElement.Add(CreateMultilineLabel($"Centroid :\n {asset.Info.Centroid}"));

            return rootElement;
        }
        
        private Label CreateMultilineLabel(string text)
        {
            // Create a new Label element
            var label = new Label(text)
            {
                style =
                {
                    // Style to allow text wrapping
                    whiteSpace = WhiteSpace.Normal, // Allows line breaks and wrapping
                    // Optional: Add padding, margins, or specific font settings
                    paddingTop = 4,
                    paddingBottom = 4,
                    paddingLeft = 8,
                    paddingRight = 8
                }
            };

            return label;
        }
    }
}