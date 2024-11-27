using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Utils.Editor
{
    public static class UIHorizontalUtils
    {
        private const float PROPERTY_LABEL_GROW = 0.35f;
        
        public static VisualElement CreateContainer() => CreateContainer(0);
        
        public static VisualElement CreateContainer(StyleLength paddingTop)
        {
            return new VisualElement
            {
                style =
                {
                    paddingTop = paddingTop,
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    alignContent = Align.Center,
                    flexGrow = 1
                }
            };
        }
        
        public static VisualElement CreatePropertyContainer()
        {
            return new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.FlexEnd,
                    alignContent = Align.Center,
                    flexGrow = 1f - PROPERTY_LABEL_GROW,
                    // backgroundColor = Color.blue
                }
            };
        }

        public static Label CreateLabel(string title, StyleLength minWidth)
        {
            return CreateLabel(title, minWidth, 10);
        }
        
        public static Label CreateLabel(string title, StyleLength minWidth, StyleLength paddingLeft)
        {
            return new Label(title)
            {
                style =
                {
                    width = minWidth,
                    paddingLeft = paddingLeft,
                    marginLeft = 5,
                    //paddingTop = 3,
                    justifyContent = Justify.FlexEnd,
                    unityTextAlign = TextAnchor.MiddleRight
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
                    // alignSelf = Align.Center,
                    paddingLeft = 5,
                    // paddingTop = 2,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    flexGrow = PROPERTY_LABEL_GROW
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
                    minWidth = 40,
                    paddingLeft = 3
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