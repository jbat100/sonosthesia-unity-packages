using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class UIHorizontalUtils
    {
        public static VisualElement CreateContainer(StyleLength paddingTop)
        {
            return new VisualElement
            {
                style =
                {
                    paddingTop = paddingTop,
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    flexGrow = 1
                }
            };
        }

        public static Label CreateLabel(string title, StyleLength minWidth)
        {
            return CreateLabel(title, minWidth, 5);
        }
        
        public static Label CreateLabel(string title, StyleLength minWidth, StyleLength paddingLeft)
        {
            return new Label(title)
            {
                style =
                {
                    width = minWidth,
                    //alignSelf = Align.Center,
                    paddingLeft = paddingLeft,
                    paddingTop = 2
                }
            };
        }
        
        public static Label CreatePropertyLabel(string title)
        {
            return new Label(title.PropertyNameToLabel())
            {
                style =
                {
                    minWidth = 120,
                    //alignSelf = Align.Center,
                    paddingLeft = 5,
                    paddingTop = 2,
                    flexGrow = 0.6f
                }
            };
        }

        public static FloatField CreateFloatField(SerializedProperty bind)
        {
            FloatField field = new FloatField
            {
                value = bind.floatValue,
                style =
                {
                    flexGrow = 0.5f,
                    minWidth = 30,
                    paddingLeft = 5
                    //alignSelf = Align.Center
                }
            };

            field.BindProperty(bind);

            return field;
        }
        
        public static EnumField CreateEnumField(SerializedProperty bind, StyleLength minWidth)
        {
            EnumField field = new EnumField
            {
                style =
                {
                    minWidth = minWidth,
                    flexGrow = 0.5f
                }
            };

            if (bind != null)
            {
                field.BindProperty(bind);
            }
            
            return field;
        }
    }
}