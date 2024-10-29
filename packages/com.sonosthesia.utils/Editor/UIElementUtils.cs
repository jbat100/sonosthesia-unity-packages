using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class UIElementUtils
    {
        public static VisualElement Separator()
        {
            return new VisualElement
            {
                style =
                {
                    height = 2,
                    backgroundColor = new StyleColor(Color.gray),
                    marginTop = 15,
                    marginBottom = 5
                }
            };
        }

        public static Label SectionLabel(string text)
        {
            return new Label(text)
            {
                style =
                {
                    paddingTop = 10,
                    paddingBottom = 10,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    fontSize = 15
                }
            };
        }
        
        public static Label TitleLabel(string text)
        {
            return new Label(text)
            {
                style =
                {
                    paddingTop = 10,
                    paddingBottom = 10,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            };
        }
    }
}