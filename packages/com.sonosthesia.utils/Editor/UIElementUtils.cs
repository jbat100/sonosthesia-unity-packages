using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class UIElementUtils
    {
        public static StyleEnum<DisplayStyle> ShowDisplayStyle(bool show) => show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}