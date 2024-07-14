using System.Collections.Generic;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sonosthesia.Palette.Editor
{
    [CustomEditor(typeof(Palette))]
    public class PaletteEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.AddSpace();
            
            root.AddField(serializedObject, "_gradient", out _, out PropertyField gradientField);
            gradientField.SetEnabled(false); // Disable editing

            //root.AddSpace();
            //Button alignGradientButton = new Button() { text = "Align Gradient to Colors" };
            //alignGradientButton.clicked += () => ((Palette)target).UpdateGradient();
            //root.Add(alignGradientButton);
            
            root.AddSeparator();
            
            root.AddField(serializedObject, "_colors");
            
            root.AddSeparator();
            
            Button importCsvButton = new Button() { text = "Import Colors from CSV" };
            importCsvButton.clicked += () => ImportColorsFromCSV((Palette)target);
            root.Add(importCsvButton);
            
            root.AddSeparator();

            TextField hexValuesField = new TextField("Hex CSV") { multiline = true };
            root.Add(hexValuesField);

            root.AddSpace();
            
            Button importTextFieldButton = new Button() { text = "Import Colors from Text Field" };
            importTextFieldButton.clicked += () => ImportColorsFromTextField((Palette)target, hexValuesField.value);
            root.Add(importTextFieldButton);
            
            return root;
        }

        private void ImportColorsFromCSV(Palette palette)
        {
            string path = EditorUtility.OpenFilePanel("Select CSV file", "", "csv");
            if (path.Length != 0)
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                List<Color> newColors = new List<Color>();

                foreach (string line in lines)
                {
                    string[] hexValues = line.Split(',');
                    foreach (string hex in hexValues)
                    {
                        string checkedHex = hex.StartsWith("#") ? hex : "#" + hex;
                        if (ColorUtility.TryParseHtmlString(checkedHex, out Color color))
                        {
                            newColors.Add(color);
                        }
                    }
                }

                palette.SetColors(newColors);
                EditorUtility.SetDirty(palette);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void ImportColorsFromTextField(Palette palette, string hexValues)
        {
            if (!string.IsNullOrEmpty(hexValues))
            {
                string[] hexArray = hexValues.Split(',');
                List<Color> newColors = new List<Color>();

                foreach (string hex in hexArray)
                {
                    if (ColorUtility.TryParseHtmlString("#" + hex.Trim(), out Color color))
                    {
                        newColors.Add(color);
                    }
                }

                palette.SetColors(newColors);
                EditorUtility.SetDirty(palette);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}