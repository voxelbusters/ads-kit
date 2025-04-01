using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor
{
	[CustomEditor(typeof(AdsKitSettings))]
	public class AdsKitSettingsInspector : SettingsObjectInspector
	{
        #region Fields

        private     AdNetworkAssetInfo[]        m_adNetworkAssets;

        private     EditorSectionInfo[]         m_serviceSections;

        private     ButtonMeta[]                m_resourceButtons;

        #endregion

        #region Properties

        public AdsKitSettings Settings => (target as AdsKitSettings);

        #endregion

        #region Base class methods

        protected override void OnEnable()
        {
            base.OnEnable();

            // Set properties
            m_adNetworkAssets   = AssetImportManager.FindAssets();
            UpdateNetworkSettingsArray(m_adNetworkAssets);
            UpdateNetworkOptions();
            UpdatePlacementOptions();

            m_resourceButtons   = new ButtonMeta[]
            {
                new ButtonMeta(label: "Tutorials",      onClick: AdsKitEditorUtility.OpenTutorialsPage),
                new ButtonMeta(label: "Discord",        onClick: AdsKitEditorUtility.OpenSupportPage),
                new ButtonMeta(label: "Write Review",	onClick: AdsKitEditorUtility.OpenProductPage),
                new ButtonMeta(label: "Subscribe",		onClick: AdsKitEditorUtility.OpenSubscribePage),
            };
            m_serviceSections   = CreateServiceSections(assetArray: m_adNetworkAssets);
        }

        protected override UnityPackageDefinition GetOwner()
        {
            return AdsKitSettings.Package;
        }

        protected override string[] GetTabNames()
        {
            return new string[]
            {
                DefaultTabs.kGeneral,
                DefaultTabs.kServices,
                DefaultTabs.kMisc,
            };
        }

        protected override EditorSectionInfo[] GetSectionsForTab(string tab)
        {
            if (tab == DefaultTabs.kServices)
            {
                return m_serviceSections;
            }
            return null;
        }

        protected override bool DrawTabView(string tab)
        {
            switch (tab)
            {
                case DefaultTabs.kGeneral:
                    DrawGeneralTabView();
                    return true;

                case DefaultTabs.kMisc:
                    DrawMiscTabView();
                    return true;

                default:
                    return false;
            }
        }

        protected override void OnSectionStatusChange(EditorSectionInfo section)
        {
            base.OnSectionStatusChange(section);

            UpdateNetworkOptions();

            // Check whether any action needs to be triggered based on new selection
            var     asset   =  (section.Tag  as AdNetworkAssetInfo);
            if (asset != null)
            {
                OnAdNetworkUsageStatusChange(section, asset);
            }

        }

        protected override void DrawFooter(string tab)
        {
            DrawHelp(tab);
            DrawErrors();   
        }

        #endregion

        #region Private methods

        private void UpdateNetworkSettingsArray(AdNetworkAssetInfo[] availableAssets)
        {
            var     networkSettingsList     = new List<AdNetworkSettings>();
            // Retain only those entries that are supported by the system
            if (Settings.NetworkSettingsArray != null)
            {
                var     retainedSettings    = Array.FindAll(array: Settings.NetworkSettingsArray,
                                                            match: (settingsItem) =>
                                                            {
                                                                return Array.Exists(availableAssets, (importerItem) => string.Equals(importerItem.NetworkId, settingsItem.NetworkId));
                                                            });
                networkSettingsList.AddRange(retainedSettings);
            }
            // Add missing entries to the current settings
            foreach (var importer in availableAssets)
            {
                string  networkId       = importer.NetworkId;
                if (networkSettingsList.Exists((item) => string.Equals(networkId, item.NetworkId))) continue;

                var     newSettings     = new AdNetworkSettings(networkId: networkId,
                                                                isEnabled: false);
                networkSettingsList.Add(newSettings);
            }

            // Update properties
            Settings.NetworkSettingsArray   = networkSettingsList.ToArray();

            EnsureChangesAreSerialized();
        }

        private void UpdateNetworkOptions()
        {
            var     enabledNetworkIds       = CollectionUtility.ConvertAll(source: Settings.NetworkSettingsArray,
                                                                           converter: (item) => item.NetworkId,
                                                                           match: (item) => item.IsEnabled);
            AdNetworkIdAttribute.SetOptions(enabledNetworkIds);
        }

        private void UpdatePlacementOptions()
        {
            var     placementNames          = CollectionUtility.ConvertAll(source: Settings.PlacementMetaArray,
                                                                           converter: (item) => item.Name);
            AdPlacementAttribute.SetOptions(placementNames);
        }

        private EditorSectionInfo[] CreateServiceSections(AdNetworkAssetInfo[] assetArray)
        {
            var     propertyGroups                  = new List<EditorSectionInfo>();
            var     networkSettingsArrayProperty    = serializedObject.FindProperty("m_networkSettingsArray");
            for (int iter = 0; iter < networkSettingsArrayProperty.arraySize; iter++)
            {
                var     element             = networkSettingsArrayProperty.GetArrayElementAtIndex(iter);
                var     elementId           = element.FindPropertyRelative("m_networkId").stringValue;
                var     asset               = Array.Find(assetArray, (item) => string.Equals(item.NetworkId, elementId));

                if (!IsAdNetworkAllowed(asset.Name)) //@@ Disabling AdColony on purpose.
                {
                    continue;
                }

                var     newPropertyGroup    = new EditorSectionInfo(displayName: asset.Name,
                                                                    description: asset.Description,
                                                                    property: element,
                                                                    drawStyle: EditorSectionDrawStyle.Default,
                                                                    drawDetailsCallback: DrawNetworkServiceSectionDetails,
                                                                    tag: asset);
                propertyGroups.Add(newPropertyGroup);
            }
            return propertyGroups.ToArray();
        }

        private void OnAdNetworkUsageStatusChange(EditorSectionInfo section, AdNetworkAssetInfo asset)
        {
            var     enabledProperty = section.Property.FindPropertyRelative("m_isEnabled");
            if (enabledProperty.boolValue && !asset.IsInstalled())
            {
                EditorApplication.delayCall += () => asset.Install();
            }
        }

        private bool IsAdNetworkAllowed(string name)
        {
            return true;
        }


        #endregion

        #region Draw methods

        private void DrawGeneralTabView()
        {
            EditorGUILayout.BeginVertical(GroupBackgroundStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isDebugBuild"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_autoLoadRetryDelay"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_autoInitOnStart"));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_loadAdMode"));
            EditorGUILayout.EndVertical();

            var     networkPreferenceSection    = new EditorSectionInfo(displayName: "Ad Network Preferences",
                                                                        property: serializedObject.FindProperty("m_networkPreferenceMeta"),
                                                                        drawStyle: EditorSectionDrawStyle.Expand,
                                                                        description: "Customize Preferred Ad Networks for Each Ad Type.");
            var     placementsSection           = new EditorSectionInfo(displayName: "Ad Placements",
                                                                        property: serializedObject.FindProperty("m_placementMetaArray"),
                                                                        drawStyle: EditorSectionDrawStyle.Expand,
                                                                        description: "Configure Ads to be Displayed.");
            var     adContentSettingsSection    = new EditorSectionInfo(displayName: "Ad Content Default Settings",
                                                                        property: serializedObject.FindProperty("m_adContentDefaultSettings"),
                                                                        drawStyle: EditorSectionDrawStyle.Expand);
            var     testDevicesSection          = new EditorSectionInfo(displayName: "Test Devices",
                                                                        property: serializedObject.FindProperty("m_testDevices"),
                                                                        drawStyle: EditorSectionDrawStyle.Expand,
                                                                        description: "Add List of Devices for Testing.");
            LayoutBuilder.DrawSection(section: networkPreferenceSection,
                                      showDetails: true,
                                      selectable: false);
            LayoutBuilder.DrawSection(section: placementsSection,
                                      showDetails: true,
                                      selectable: false);
            LayoutBuilder.DrawSection(section: adContentSettingsSection,
                                      showDetails: true,
                                      selectable: false);
            LayoutBuilder.DrawSection(section: testDevicesSection,
                                      showDetails: true,
                                      selectable: false);
            UpdatePlacementOptions();
        }

        private void DrawMiscTabView()
        {
            DrawButtonList(m_resourceButtons);
        }

        private void DrawNetworkServiceSectionDetails(EditorSectionInfo section)
        {
            var     asset               = section.Tag as AdNetworkAssetInfo;
            bool    originalGUIState    = GUI.enabled;

            GUI.enabled     = section.IsEnabled;
            EditorGUI.indentLevel++;
            LayoutBuilder.BeginVertical();
            DrawNetworkSettingsProperties(asset.NetworkId, section);
            LayoutBuilder.EndVertical();
            EditorGUI.indentLevel--;
            GUI.enabled     = originalGUIState;

            // Additional actions
            if (asset.IsInstalled())
            {
                if (!section.IsEnabled && GUILayout.Button("Uninstall"))
                {
                    if (GUILayout.Button("Do you want to uninstall ad network as well?"))
                    {
                        EditorApplication.delayCall += () => asset.Uninstall(true);
                    }
                    else
                    {
                        EditorApplication.delayCall += () => asset.Uninstall(false);
                    }
                }
            }
            else if (GUILayout.Button("Install"))
            {
                EditorApplication.delayCall += () => asset.Install();
            }
        }

        private void DrawNetworkSettingsProperties(string networkId, EditorSectionInfo section)
        {
            var     apiKeyProperty          = section.Property.FindPropertyRelative("m_runtimeApiKey");
            var     apiKeyOverridesProperty = section.Property.FindPropertyRelative("m_runtimeApiKeyOverrides");

            if (string.Equals(networkId, AdNetworkServiceId.kAppLovin))
            {
                DrawStringProperty("Sdk Key", apiKeyProperty);
            }
            else if (string.Equals(networkId, AdNetworkServiceId.kGoogleMobileAds))
            {
                DrawStringProperty("iOS App Id", apiKeyOverridesProperty.FindPropertyRelative("m_ios"));
                DrawStringProperty("Android App Id", apiKeyOverridesProperty.FindPropertyRelative("m_android"));
            }
            else if (string.Equals(networkId, AdNetworkServiceId.kLevelPlay))
            {
                DrawStringProperty("iOS App Key", apiKeyOverridesProperty.FindPropertyRelative("m_ios"));
                DrawStringProperty("Android App Key", apiKeyOverridesProperty.FindPropertyRelative("m_android"));
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            //EditorGUILayout.LabelField($"If you want to serve a placement with {networkId}, link placement id with it's corresponding ad unit id's obtained from {networkId} dashabord below.", CustomEditorStyles.OptionsLabel());
            EditorGUILayout.HelpBox($"If you want to serve an ad at a placement with {networkId}, link placement id with it's corresponding ad unit id's obtained from {networkId} dashabord below.", MessageType.None);
            EditorGUILayout.PropertyField(section.Property.FindPropertyRelative("m_adMetaArray"));
        }

        private static void DrawStringProperty(string displayName, SerializedProperty property)
        {
           property.stringValue = EditorGUILayout.TextField(displayName, property.stringValue);
        }

        private void DrawHelp(string tab)
        {
            if (tab.Equals(DefaultTabs.kServices))
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Info", NormalLabelStyle);
                EditorGUILayout.HelpBox("On enabling the Ad Network service, installation starts automatically. You can setup the Ad Network service by clicking on the enabled service.", MessageType.None);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawErrors()
        {
            var errorLogListProperty = serializedObject.FindProperty("m_errorLogList");
            var size = errorLogListProperty.arraySize;

            if (size > 0)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Errors", NormalLabelStyle);
                for (int i = 0; i < size; i++)
                {
                    var value = errorLogListProperty.GetArrayElementAtIndex(i).stringValue;
                    EditorGUILayout.HelpBox(value, MessageType.Error);
                }
                EditorGUILayout.EndVertical();
            }
        }

        #endregion
    }
}