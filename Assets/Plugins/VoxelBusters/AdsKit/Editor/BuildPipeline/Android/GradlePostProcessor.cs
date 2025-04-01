#if UNITY_ANDROID
using UnityEditor.Android;

namespace VoxelBusters.AdsKit.Editor.Build.Android
{
    class GradlePostProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder { get { return 0; } }
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!AdsKitSettingsEditorUtility.SettingsExists)
            {
                return;
            }

            AdsKitSettings settings = AdsKitSettingsEditorUtility.DefaultSettings;
            string manifestPath     = string.Format("{0}/{1}/{2}", path, AdsKitPackageLayout.AndroidProjectFolderName, "AndroidManifest.xml");
            AndroidManifestGenerator.GenerateManifest(settings, manifestPath);
        }
    }
}
#endif