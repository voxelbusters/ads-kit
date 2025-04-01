using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary.Editor;
using System;

namespace VoxelBusters.AdsKit.Editor
{
    [CustomEditor(typeof(BannerAdOptionsAsset))]
    public class BannerAdOptionsAssetInspector : UnityEditor.Editor
    {
        #region Fields

        private     SerializedProperty      m_positionPresetProperty;

        private     SerializedProperty      m_absolutePositionProperty;

        private     SerializedProperty      m_sizeTypeProperty;

        private     SerializedProperty      m_customSizeProperty;

        private     string[]                m_sizeTypeOptions;

        private     int                     m_selectedSizeTypeIndex;

        private     int                     m_customSizeTypeIndex;

        #endregion

        #region Base class methods

        private void OnEnable()
        {
            // Set properties
            m_positionPresetProperty        = serializedObject.FindProperty("m_positionPreset");
            m_absolutePositionProperty      = serializedObject.FindProperty("m_absolutePosition");
            m_sizeTypeProperty              = serializedObject.FindProperty("m_sizeType");
            m_customSizeProperty            = serializedObject.FindProperty("m_customSize");
            m_sizeTypeOptions               = new string[]
            {
                AdSizeName.Banner,
                AdSizeName.LargeBanner,
                AdSizeName.MediumRectangle,
                AdSizeName.FullBanner,
                AdSizeName.Leaderboard,
                "Custom"
            };
            m_selectedSizeTypeIndex     = System.Array.FindIndex(m_sizeTypeOptions, (item) => string.Equals(item, m_sizeTypeProperty.stringValue));
            m_customSizeTypeIndex       = m_sizeTypeOptions.Length - 1;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw properties
            
            var isAbsolutePositionSpeicified = IsAbsolutePositionSpeicified(m_absolutePositionProperty);

            GUI.enabled = !isAbsolutePositionSpeicified;
            EditorGUILayout.PropertyField(m_positionPresetProperty);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(m_absolutePositionProperty);
            
            EditorLayoutUtility.StringPopup(m_sizeTypeProperty,
                                            ref m_selectedSizeTypeIndex,
                                            m_sizeTypeOptions);
            if (m_selectedSizeTypeIndex == m_customSizeTypeIndex)
            {
                EditorGUILayout.PropertyField(m_customSizeProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region 

        private bool IsAbsolutePositionSpeicified(SerializedProperty property)
        {
            var value = property.vector2IntValue;
            return !(value.x == 0 && value.y == 0);
        }

        #endregion
    }
}