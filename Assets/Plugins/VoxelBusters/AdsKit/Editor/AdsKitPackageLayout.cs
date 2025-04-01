namespace VoxelBusters.AdsKit.Editor
{
    internal static class AdsKitPackageLayout
    {
        public static string ExtrasPath => "Assets/Plugins/VoxelBusters/AdsKit/Essentials";

        public static string EditorExtrasPath => $"{ExtrasPath}/Editor";

        public static string IosPluginPath => $"{ExtrasPath}/Plugins/iOS";
        public static string AndroidPluginPath => $"Assets/Plugins/VoxelBusters/AdsKit/Plugins/Android";
        public static string AndroidProjectRootNamespace => "com.voxelbusters.adskit";
        public static string AndroidProjectFolderName => $"{AndroidProjectRootNamespace}.androidlib";
        public static string AndroidProjectPath => $"{AndroidPluginPath}/{AndroidProjectFolderName}";
    }
}