using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace Sonosthesia.Scaffold.Editor
{
    public enum SplineGeneratorType
    {
        Line,
        Spiral,
        Polygon,
        Helix
    }
    
    public class SplineGeneratorEditor : EditorWindow
    {
        private const string SPLINE_CONTAINER_FIELD = "SplineContainerField";
        private const string SPLINE_INDEX_FIELD = "SplineIndexField";
        private const string HEADER_ROOT = "HeaderRoot";
        private const string GENERATOR_ROOT = "GeneratorRoot";
        private const string GENERATE_BUTTON = "GenerateButton";

        private const SplineGeneratorType DefaultGeneratorType = SplineGeneratorType.Spiral;
        
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;
        
        [SerializeField]
        private VisualTreeAsset m_LineTreeAsset;

        [SerializeField] 
        private VisualTreeAsset m_SpiralTreeAsset;
        
        [SerializeField]
        private VisualTreeAsset m_PolygonTreeAsset;
        
        [SerializeField]
        private VisualTreeAsset m_HelixTreeAsset;
        
        private VisualElement _currentGeneratorElement;
        private SplineGenerator _currentGenerator;

        [MenuItem("Window/Sonosthesia/SplineGeneratorEditor")]
        public static void ShowExample()
        {
            SplineGeneratorEditor wnd = GetWindow<SplineGeneratorEditor>();
            wnd.titleContent = new GUIContent("Spline Generator");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            // Instantiate UXML
            VisualElement visualTree = m_VisualTreeAsset.Instantiate();
            root.Add(visualTree);

            bool success = rootVisualElement.TryGetElementByName(HEADER_ROOT, out VisualElement headerRoot) &
                           rootVisualElement.TryGetElementByName(SPLINE_CONTAINER_FIELD, out ObjectField splineContainerField) &
                           rootVisualElement.TryGetElementByName(SPLINE_INDEX_FIELD, out IntegerField splineIndexField) &
                           rootVisualElement.TryGetElementByName(GENERATE_BUTTON, out Button generateButton);

            if (!success)
            {
                return;
            }
            
            EnumField generatorTypeField = new EnumField("Generator Type", DefaultGeneratorType);
            headerRoot.Add(generatorTypeField);
            
            Switch(DefaultGeneratorType);
            
            // Mirror the value of the UXML field into the C# field.
            generatorTypeField.RegisterCallback<ChangeEvent<Enum>>((evt) =>
            {
                Enum value = evt.newValue;
                SplineGeneratorType castValue = (SplineGeneratorType)value;
                Debug.Log($"{this} changed value to {value} cast {castValue}");
                Switch(castValue);
            });

            generateButton.clicked += () =>
            {
                SplineContainer splineContainer = splineContainerField.value as SplineContainer;
                int splineIndex = Mathf.Max(splineIndexField.value, 0);

                if (splineContainer == null)
                {
                    return;
                }
                
                Spline spline = _currentGenerator?.Generate();

                List<Spline> splines = splineContainer.Splines.ToList();
                if (splines.Count > splineIndex)
                {
                    splines[splineIndex] = spline;
                }
                else
                {
                    splines.Add(spline);
                }

                splineContainer.Splines = splines;
            };
        }

        private VisualTreeAsset AssetForType(SplineGeneratorType generatorType)
        {
            return generatorType switch
            {
                SplineGeneratorType.Line => m_LineTreeAsset,
                SplineGeneratorType.Spiral => m_SpiralTreeAsset,
                SplineGeneratorType.Polygon => m_PolygonTreeAsset,
                _ => throw new NotSupportedException(generatorType.ToString())
            };
        }

        private SplineGenerator GeneratorForType(SplineGeneratorType generatorType)
        {
            return generatorType switch
            {
                SplineGeneratorType.Spiral => new SplineSpiralGenerator(),
                _ => null
            };
        }

        private void Switch(SplineGeneratorType generatorType)
        {
            if (!rootVisualElement.TryGetElementByName(GENERATOR_ROOT, out VisualElement generatorRoot))
            {
                return;
            }

            if (_currentGeneratorElement != null)
            {
                generatorRoot.Remove(_currentGeneratorElement);
            }

            VisualTreeAsset asset = AssetForType(generatorType);

            if (asset == null)
            {
                Debug.LogError($"{this} null {nameof(VisualTreeAsset)} for {generatorType}");
                return;
            }

            VisualElement generatorElement = asset.Instantiate();
            generatorRoot.Add(generatorElement);
            _currentGeneratorElement = generatorElement;
            
            _currentGenerator = GeneratorForType(generatorType);
            if (_currentGenerator != null)
            {
                _currentGenerator.Setup(generatorElement);
            }
        }
    }
    
}

