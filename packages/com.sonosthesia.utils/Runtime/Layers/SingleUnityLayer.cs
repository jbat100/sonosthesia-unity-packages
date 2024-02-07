using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
#if UNITY_EDITOR

    using UnityEditor;
    
    [CustomPropertyDrawer(typeof(SingleUnityLayer))]
    public class SingleUnityLayerPropertyDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, GUIContent.none, _property);
            SerializedProperty layerIndex = _property.FindPropertyRelative("_layerIndex");
            _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
            if (layerIndex != null)
            {
                layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
            }
            EditorGUI.EndProperty( );
        }
    }
    
#endif
    
    [Serializable]
    public class SingleUnityLayer
    {
        [SerializeField]
        private int _layerIndex = 0;
        public int LayerIndex
        {
            get => _layerIndex;
            set
            {
                if (value is > 0 and < 32)
                {
                    _layerIndex = value;
                }
            }
        }
        
        public int Mask => 1 << _layerIndex;
    }
}