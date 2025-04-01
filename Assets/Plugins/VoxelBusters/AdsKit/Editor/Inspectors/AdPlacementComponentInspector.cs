using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor
{
    [CustomEditor(typeof(AdPlacementComponent))]
    public class AdPlacementComponentInspector : UnityEditor.Editor
    {
        #region Unity methods

        private void OnEnable()
        {
            UpdateStaticOptions();
        }

        #endregion

        #region Private methods

        private void UpdateStaticOptions()
        {
            // Set properties
            string[]    placementOptions;
            if (AdsKitSettingsEditorUtility.TryGetDefaultSettings(out AdsKitSettings settings))
            {
                placementOptions    = CollectionUtility.ConvertAll(source: settings.PlacementMetaArray,
                                                                   converter: (item) => item.Name);
            }
            else
            {
                placementOptions    = new string[0];
            }
            AdPlacementAttribute.SetOptions(placementOptions);
        }

        #endregion
    }
}