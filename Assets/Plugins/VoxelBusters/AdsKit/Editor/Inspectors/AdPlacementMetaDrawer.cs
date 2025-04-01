using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.AdsKit.Editor
{
    [CustomPropertyDrawer(typeof(AdPlacementMeta))]
    public class AdPlacementMetaDrawer : PropertyDrawer
    {
        #region Base class methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position                = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var     indent          = EditorGUI.indentLevel;
            EditorGUI.indentLevel   = 0;

            var     nameProperty    = property.FindPropertyRelative("m_name");
            var     adTypeProperty  = property.FindPropertyRelative("m_adType");
            var     autoLoadProperty= property.FindPropertyRelative("m_autoLoad");
            var     optionsProperty = property.FindPropertyRelative("m_optionsObject");

            // Calculate rects
            bool    canShowOptions  = CanShowOptionsPropertyForAdType(adTypeProperty, out System.Type optionsType);
            float   optionsOffset   = canShowOptions ? 10f : 0f;
            float   optionsWidth    = canShowOptions ? 120f : 0f;
            float   reservedWidth   = 220f + optionsOffset + optionsWidth;
            var     typeRect        = new Rect(position.x, position.y, 120, position.height);
            var     autoLoadRectPrefix  = new Rect(position.x + 130, position.y, 20, position.height);
            var     autoLoadRect        = new Rect(position.x + 200, position.y, 20, position.height);
            var     nameRect        = new Rect(position.x + 220, position.y, position.width - reservedWidth, position.height);
            var     optionsRect     = new Rect(nameRect.xMax + optionsOffset, position.y, optionsWidth, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(typeRect, adTypeProperty, GUIContent.none);
            EditorGUI.PrefixLabel(autoLoadRectPrefix, new GUIContent("Auto Load"));
            EditorGUI.PropertyField(autoLoadRect, autoLoadProperty, new GUIContent(text: string.Empty, tooltip: autoLoadProperty.tooltip));
            EditorGUI.PropertyField(nameRect, nameProperty, GUIContent.none);
            if (canShowOptions)
            {
                EditorGUI.ObjectField(optionsRect, optionsProperty, optionsType, GUIContent.none);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        #endregion

        #region Private methods

        private bool CanShowOptionsPropertyForAdType(SerializedProperty adTypeProperty, out System.Type type)
        {
            int     enumIndex   = adTypeProperty.enumValueIndex;
            // Banner
            if (enumIndex == 0)
            {
                type    = typeof(BannerAdOptionsAsset);
                return true;
            }
            else
            {
                type    = null;
                return false;
            }
        }

        #endregion
    }
}