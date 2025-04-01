using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The wrapper class used to interact with the AdsKit SDK.
    /// </summary>
    [IncludeInDocs]
    public static class AdsManager
    {
        #region Static properties

        internal static AdsManagerImpl SharedInstance { get; private set; }

        /// <summary>
        /// Returns true if the SDK is initialised successfully, and false if it isn't.
        /// </summary>
        public static bool IsInitialised => IsAvailable() && SharedInstance != null && SharedInstance.IsInitialised;

        /// <summary>
        /// Returns true if the SDK is initialised/initialising.
        /// </summary>
        public static bool IsInitialisedOrWillChange => IsAvailable() && SharedInstance.IsInitialisedOrWillChange;

        /// <summary>
        /// Returns true if the SDK is is in debug mode.
        /// </summary>
        public static bool IsDebugBuild => IsAvailable() && SharedInstance.IsDebugBuild;

        /// <summary>
        /// Returns AdMob adapter reference.
        /// </summary>
        public static AdNetworkAdapter AdMob => SharedInstance?.AdMob;
        
        /// <summary>
        /// Returns AppLovin adapter reference.
        /// </summary>
        public static AdNetworkAdapter AppLovin => SharedInstance?.AppLovin;
        
        /// <summary>
        /// Returns LevelPlay adapter reference.
        /// </summary>
        public static AdNetworkAdapter LevelPlay => SharedInstance?.LevelPlay;

        
        #endregion

        #region Static methods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadManager()
        {
            // Create manager object
            CallbackDispatcher.Initialize();
            var     settings    = AdsKitSettings.Instance;

            if(settings.IsDebugBuild)
            {
                DebugLogger.SetLogLevel(DebugLogger.LogLevel.Info, new string[] { AdsKitSettings.Domain });
                DebugLogger.SetLogLevel(DebugLogger.LogLevel.Info);
            }

            if (settings.IsEnabled)
            {
                SharedInstance  = AdsManagerImpl.Create(name: "AdManager(Singleton)",
                                                        settings: settings);
            }
        }

        /// <summary>
        /// Initialises the SDK.
        /// </summary>
        /// <param name="applicationPrivacyConfiguration">Privacy configuration obtained after user's consent.</param>
        /// <returns>Async operation.</returns>
        public static IAsyncOperation<InitialiseResult> Initialise(IConsentFormProvider consentFormProvider)
        {
            Assert.IsArgNotNull(consentFormProvider, "You must provide a consent form provider(IConsentFormProvider) to initialise Ads Kit.");

            return SharedInstance?.Initialise(consentFormProvider);
        }


        /// <summary>
        /// Adds a listener that will recieve Ads lifecycle callbacks. This method allows you to register multiple listeners.
        /// </summary>
        /// <param name="listener">A listener for Ads lifecycle callbacks.</param>
        public static void RegisterListener(IAdLifecycleEventListener listener)
        {
            var     manager = SharedInstance;
            if (manager == null) return;

            manager.RegisterListener(listener as IInitialiseEventListener);
            manager.RegisterListener(listener as ILoadAdEventListener);
            manager.RegisterListener(listener as IShowAdEventListener);
        }

        /// <summary>
        /// Allows you to remove an active listener.
        /// </summary>
        /// <param name="listener">A listener for Ads lifecycle callbacks.</param>
        public static void UnregisterListener(IAdLifecycleEventListener listener)
        {
            var     manager = SharedInstance;
            if (manager == null) return;

            manager.UnregisterListener(listener as IInitialiseEventListener);
            manager.UnregisterListener(listener as ILoadAdEventListener);
            manager.UnregisterListener(listener as IShowAdEventListener);
        }

        #endregion

        #region Ad methods

        /// <summary>
        /// Returns the state of the specified placement.
        /// </summary>
        /// <param name="placement">The unique identifier for a specific Placement</param>
        /// <returns>Placement state.</returns>
        public static AdPlacementState GetAdPlacementState(string placement)
        {
            if (!IsAvailable()) return AdPlacementState.Unknown;

            return SharedInstance.GetAdPlacementState(placement);
        }

        /// <summary>
        /// Loads ad content for a specified Placement.
        /// </summary>
        /// <param name="placement">The unique identifier for a specific Placement.</param>
        /// <param name="options">Specify the addtional options specific to the Placement.</param>
        /// <returns>Async-operation reference to the load.</returns>
        public static LoadAdRequest LoadAd(string placement, AdContentOptions options = null)
        {
            if (!IsAvailable()) return null;

            return SharedInstance.LoadAd(placement, options);
        }

        /// <summary>
        /// Displays an ad in the specific Ad Unit or Placement if it is ready.
        /// </summary>
        /// <param name="placement">The unique identifier for a specific Placement.</param>
        /// <returns>Async-operation reference to the show.</returns>
        public static AdContent ShowAd(string placement)
        {
            if (!IsAvailable()) return null;

            return SharedInstance.ShowAd(placement);
        }

        /// <summary>
        /// Dismisses the specified ad view. This method works for Banner type.
        /// </summary>
        /// <param name="placement">The unique identifier for a specific Placement.</param>
        /// <param name="destroy">Set true if ad needs to get destroyed.</param>
        public static void HideAd(string placement, bool destroy = false)
        {
            if (!IsAvailable()) return;

            SharedInstance.HideAd(placement, destroy);
        }

        #endregion

        #region Setter methods

        /// <summary>
        /// Sets the Placement details. This meta information will be used to determine the specifics of the Placement such as AdType, auto-load etc
        /// </summary>
        /// <param name="array">An array of the Placement meta.</param>
        public static void SetPlacementMeta(params AdPlacementMeta[] array)
        {
            if (!IsAvailable()) return;

            SharedInstance.SetPlacementMeta(array);
        }

        /// <summary>
        /// Sets the privacy configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void SetPrivacyConfiguration(ApplicationPrivacyConfiguration config)
        {
            if (!IsAvailable()) return;

            SharedInstance.SetPrivacyConfiguration(config);
        }

        /// <summary>
        /// Sets the additional information of the application user.
        /// </summary>
        /// <param name="user">The user.</param>
        public static void SetUser(User user)
        {
            if (!IsAvailable()) return;

            SharedInstance.SetUser(user);
        }

        /// <summary>
        /// Sets the orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public static void SetOrientation(ScreenOrientation orientation)
        {
            if (!IsAvailable()) return;

            SharedInstance.SetOrientation(orientation);
        }

        #endregion

        #region Private static methods

        private static bool IsAvailable()
        {
            if (!AdsKitSettings.Instance.IsEnabled)
            {
                DebugLogger.Log(AdsKitSettings.Domain, "The requested operation could not be completed because AdsKit is disabled. Please enable the service from the project settings.");
                return false;
            }
            return true;
        }

        #endregion
    }
}