using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class UIElementUtils
    {
        public static StyleEnum<DisplayStyle> ShowDisplayStyle(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;

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