using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor
{
    [CustomPropertyDrawer(typeof(AdPlacementAttribute))]
    public class AdPlacementAttributeDrawer : StringPopupAttributeDrawer
    {

        public AdPlacementAttributeDrawer()
        {
            UpdateStaticOptions(); //@@ Needs review (If this is fine to call in OnGUI everytime of fine in constructor?)
        }

        #region Private methods

        private void UpdateStaticOptions()
        {
            // Set properties
            string[] placementOptions;
            if (AdsKitSettingsEditorUtility.TryGetDefaultSettings(out AdsKitSettings settings))
            {
                placementOptions = CollectionUtility.ConvertAll(source: settings.PlacementMetaArray,
                                                                   converter: (item) => item.Name);
            }
            else
            {
                placementOptions = new string[0];
            }
            AdPlacementAttribute.SetOptions(placementOptions);
        }

        #endregion
    }
}