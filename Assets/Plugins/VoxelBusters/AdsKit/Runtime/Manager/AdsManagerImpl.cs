using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    public class AdsManagerImpl : MonoBehaviourZ
    {
        #region Fields

        [SerializeField]
        private     AdsKitSettings                                      m_settings;

        private     AdNetworkSettings[]                                 m_networkSettingsArray;

        private     AdNetworkPreferenceMeta                             m_networkPreferenceMeta;

        private     AdPlacementMeta[]                                   m_placementMetaArray;

        private     DeviceCollection                                    m_testDevices;

        private     AdNetworkAdapter[]                                  m_activeNetworks;

        private     Dictionary<AdType, AdNetworkAdapter[]>              m_networkPreferenceMap;

        private     AdContentSettings                                   m_adContentDefaultSettings;

        private     InitialiseAdNetworksOperation                       m_initAdNetworksOperation;

        private     InitialiseAfterConsentOperation                     m_initConsentOperation;

        private     List<LoadAdOperation>                               m_inprogressLoadOperations;

        private     List<LoadAdOperation>                               m_completedLoadOperations;

        private     List<AdContent>                                     m_activeAdContents;

        private     bool                                                m_isDebugBuild;

        private     ApplicationPrivacyConfiguration                     m_privacyConfig;

        private     User                                                m_user;

        private     UserSettings                                        m_userSettings;

        private     ScreenOrientation?                                  m_orientation;

        private     EventHandlerCollection<IInitialiseEventListener>    m_initListeners;

        private     EventHandlerCollection<ILoadAdEventListener>        m_loadAdListeners;

        private     EventHandlerCollection<IShowAdEventListener>        m_showAdListeners;

        [ClearOnReload(ClearOnReloadOption.Default)]
        private static readonly object s_objectLock = new object();

        #endregion

        #region Properties

        public bool IsInitialised { get; private set; }

        public bool IsInitialisedOrWillChange => IsInitialised || (m_initAdNetworksOperation != null) || (m_initConsentOperation != null);

        public bool IsDebugBuild => m_isDebugBuild;

        public LoadAdMode PreferredLoadAdMode { get; set; }

        public AdNetworkAdapter AdMob => GetAdNetworkWithId(networkId: AdNetworkServiceId.kGoogleMobileAds);
        
        public AdNetworkAdapter AppLovin => GetAdNetworkWithId(networkId: AdNetworkServiceId.kAppLovin);
        
        public AdNetworkAdapter LevelPlay => GetAdNetworkWithId(networkId: AdNetworkServiceId.kLevelPlay);


        #endregion

        #region Create methods

        public static AdsManagerImpl Create(string name, AdsKitSettings settings)
        {
            var     newGO   = GameObjectUtility.CreateGameObject(name,
                (instance) =>
                {
                    var     manager     = instance.AddComponent<AdsManagerImpl>();
                    manager.m_settings  = settings;
                });
            return newGO.GetComponent<AdsManagerImpl>();
        }

        public static AdNetworkAdapter[] ConvertAdNetworkIdsToObjects(AdsManagerImpl manager, string[] ids)
        {
            var     instanceList    = new List<AdNetworkAdapter>();
            foreach (var currentId in ids)
            {
                var     instance    = manager.GetAdNetworkWithId(currentId);
                if (instance == null) continue;

                instanceList.Add(instance);
            }
            return instanceList.ToArray();
        }

        #endregion

        #region Base class methods

        protected override void Init()
        {
            base.Init();

            // Set component properties
            IsInitialised               = false;
            m_placementMetaArray        = new AdPlacementMeta[0];
            m_activeNetworks            = new AdNetworkAdapter[0];
            m_networkPreferenceMap      = new Dictionary<AdType, AdNetworkAdapter[]>(capacity: 8);
            m_inprogressLoadOperations  = new List<LoadAdOperation>();
            m_completedLoadOperations   = new List<LoadAdOperation>();
            m_activeAdContents          = new List<AdContent>();
            m_initListeners             = new EventHandlerCollection<IInitialiseEventListener>();
            m_loadAdListeners           = new EventHandlerCollection<ILoadAdEventListener>();
            m_showAdListeners           = new EventHandlerCollection<IShowAdEventListener>();

            // Mark the instance as persistent object
            DontDestroyOnLoad(gameObject);
        }

        protected override void Start()
        {
            base.Start();

            // Auto initialize the component - Currently AutoInitOnStart is intentionally disabled and its value will always be false.
            if (!IsInitialisedOrWillChange && m_settings && m_settings.AutoInitOnStart)
            {
                if (!m_settings.IsEnabled)
                {
                    DebugLogger.LogWarning(AdsKitSettings.Domain, "We have observed that application has marked AdsKit as disabled.");
                    return;
                }
                Initialise(m_settings, null);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            // Forward the pause state information to all the networks
            ExecuteNetworkFunction((network) => network.SetPaused(pause));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        #endregion

        #region Init methods

        public IAsyncOperation<InitialiseResult> Initialise(IConsentFormProvider consentFormProvider)
        {
            return Initialise(settings: m_settings, consentFormProvider);
        }

        public IAsyncOperation<InitialiseResult> Initialise(AdsKitSettings settings, IConsentFormProvider consentFormProvider)
        {
            // Guard case
            Assert.IsArgNotNull(settings, nameof(settings));
            Assert.IsArgNotNull(consentFormProvider, nameof(IConsentFormProvider));

            return Initialise(networkSettingsArray: settings.NetworkSettingsArray,
                              networkPreferenceMeta: settings.NetworkPreferenceMeta,
                              placementMetaArray: settings.PlacementMetaArray,
                              loadAdMode: settings.LoadAdMode,
                              adContentDefaultSettings: settings.AdContentDefaultSettings,
                              testDevices: settings.TestDevices,
                              consentFormProvider: consentFormProvider,
                              isDebugBuild: settings.IsDebugBuild);
        }

        public IAsyncOperation<InitialiseResult> Initialise(AdNetworkSettings[] networkSettingsArray,
                                                            AdNetworkPreferenceMeta networkPreferenceMeta,
                                                            AdPlacementMeta[] placementMetaArray,
                                                            AdContentSettings adContentDefaultSettings,
                                                            DeviceCollection testDevices,
                                                            LoadAdMode loadAdMode,
                                                            IConsentFormProvider consentFormProvider,
                                                            bool isDebugBuild
                                                            )
        {
            // Guard case
            Assert.IsNotNullOrEmpty(networkSettingsArray, nameof(networkSettingsArray));

            if (IsInitialisedOrWillChange) return null;

            // Cache information
            m_networkSettingsArray      = Array.FindAll(networkSettingsArray, (item) => item.IsEnabled);
            m_networkPreferenceMeta     = networkPreferenceMeta;
            m_placementMetaArray        = placementMetaArray;
            m_adContentDefaultSettings  = adContentDefaultSettings;
            m_testDevices               = testDevices;
            PreferredLoadAdMode         = loadAdMode;
            m_isDebugBuild              = isDebugBuild || Application.isEditor;

            // Start consent operation and then initialise ad networks
            m_initConsentOperation = new InitialiseAfterConsentOperation(consentFormProvider, config =>
            {
                SetPrivacyConfiguration(config);
                m_initConsentOperation = null;

                // Start ad networks init operation
                m_initAdNetworksOperation = new InitialiseAdNetworksOperation(manager: this,
                                                                      networkSettingsArray: m_networkSettingsArray,
                                                                      privacyConfiguration: config,
                                                                      isDebugBuild: m_isDebugBuild);
                m_initAdNetworksOperation.Start();
                return m_initAdNetworksOperation;
            });

            //This gets triggered by InitialiseAdNetworksOperation (from internal oncomplete event as well as if consent fails in InitialiseAfterConsentOperation)
            m_initConsentOperation.OnComplete += OnInitialiseComplete;            
            SurrogateCoroutine.WaitForEndOfFrameAndInvoke(() => m_initConsentOperation.Start());
            return m_initConsentOperation;
        }

        #endregion

        #region Register methods

        public void RegisterListener(IInitialiseEventListener listener)
        {
            EnsureInitialised();

            m_initListeners.Add(listener);
        }

        public void RegisterListener(ILoadAdEventListener listener)
        { 
            EnsureInitialised();

            m_loadAdListeners.Add(listener);
        }

        public void RegisterListener(IShowAdEventListener listener)
        { 
            EnsureInitialised();

            m_showAdListeners.Add(listener);
        }

        public void UnregisterListener(IInitialiseEventListener listener)
        { 
            EnsureInitialised();

            m_initListeners.Remove(listener);
        }

        public void UnregisterListener(ILoadAdEventListener listener)
        { 
            EnsureInitialised();

            m_loadAdListeners.Remove(listener);
        }

        public void UnregisterListener(IShowAdEventListener listener)
        { 
            EnsureInitialised();

            m_showAdListeners.Remove(listener);
        }

        #endregion

        #region Ad methods

        public AdContent CreateAdContent(AdType adType,
                                         string placement,
                                         AdNetworkAdapter provider = null,
                                         AdContentOptions contentOptions = null)
        {
            // Factory method to create appropriate instance
            switch (adType)
            {
                case AdType.Banner:
                    return new BannerAdContent(placement: placement,
                                               manager: this,
                                               provider: provider,
                                               options: contentOptions);

                case AdType.Interstitial:
                    return new InterstitialAdContent(placement: placement,
                                                     manager: this,
                                                     provider: provider);

                case AdType.Video:
                    return new VideoAdContent(placement: placement,
                                              manager: this,
                                              provider: provider);

                case AdType.RewardedVideo:
                    return new RewardedVideoAdContent(placement: placement,
                                               manager: this,
                                               provider: provider);

                default:
                    throw VBException.SwitchCaseNotImplemented(adType);
            }
        }

        internal AdContentOptions GetDefaultAdContentOptions(AdType adType)
        {
            switch (adType)
            {
                case AdType.Banner:
                    AdContentOptions bannerOptions = null;
                    try
                    {
                        bannerOptions = (BannerAdOptions)m_adContentDefaultSettings.Banner;
                    }
                    catch
                    {
                        DebugLogger.LogError(AdsKitSettings.Domain, "Failed to get default banner options. Check in Ads Kit Settings if content options has any missing reference.");
                    }

                    return bannerOptions;

                default:
                    return null;
            }
        }

        public AdPlacementState GetAdPlacementState(string placement)
        {
            // Guard case
            if (!PreprocessAdRequest(out Error error)) return AdPlacementState.Unknown;

            // Find meta infomation
            var     loadOp  = GetLoadAdOperation(placement);
            if (loadOp == null)
            {
                return AdPlacementState.NotAvailable;
            }
            else if (loadOp.IsDone)
            {
                return AdPlacementState.Ready;
            }
            else
            {
                return AdPlacementState.Waiting;
            }
        }

        public LoadAdRequest LoadAd(string placement,
                                    AdContentOptions options = null)
        {

            // Clean up if any existing loaded ad
            CleanupUsedAds(placement);

            // Create new operation if required
            var     loadOp  = GetLoadAdOperation(placement);
            if (loadOp == null)
            {
                loadOp      = CreateLoadAdOperation(placement: placement,
                                                    options: options);
            }

            return new LoadAdRequest(loadOp);
        }

        public AdContent ShowAd(string placement)
        {
            // Find meta infomation
            var     placementMeta   = GetAdPlacementMetaWithPlacement(placement);
            if (placementMeta == null)
            {
                var description = $"Could not find metadata for placement id: {placement}";
                SendShowAdFailEvent(placement, new Error(AdsKitSettings.Domain, AdErrorCode.kShowFail, description));
                return null;
            }
                

            return ShowAd(adType: placementMeta.AdType,
                          placement: placementMeta.Name);
        }

        public AdContent ShowAd(AdType adType, string placement)
        {
            var     adContent   = GetActiveAdContent(placement);
            if (adContent == null)
            {
                var     completedLoadOp = FindLoadAdOperationForPlacement(m_completedLoadOperations, placement);
                if (completedLoadOp == null)
                {
                    var description = $"No loaded content available for placement id: {placement}, ad type: {adType}. You first need to load the ad to show it. If you have already enabled auto load, most likely the ad loading failed either due to network or missing privacy configuration status (consent)";
                    SendShowAdFailEvent(placement, new Error(AdsKitSettings.Domain, AdErrorCode.kShowFail, description));
                    return null;
                }


                var     provider        = (completedLoadOp != null)
                    ? GetAdNetworkWithId(completedLoadOp.Result.PreferredAdProvider)
                    : null;

                var     newContent      = CreateAdContent(adType,
                                                          placement, 
                                                          provider,
                                                          completedLoadOp.ContentOptions);
                m_activeAdContents.Add(newContent);
                if (newContent.IsReady)
                {
                    newContent.ShowAd();
                }

                adContent   = newContent;
            }
            else if(!adContent.IsShowingOrWillShow)
            {
                adContent.ShowAd();
            }

            return adContent;
        }

        public void HideAd(string placement, bool destroy = false)
        {
            var     adContent   = GetActiveAdContent(placement);
            if (adContent == null)
            {
                DebugLogger.LogWarning(AdsKitSettings.Domain, $"The operation could not be completed because there is no ad content currently displayed at placement {placement}.");
                return;
            }
            if ((adContent is BannerAdContent) == false)
            {
                DebugLogger.LogWarning(AdsKitSettings.Domain, $"The operation could not be completed because ad content does not support specified request.");
                return;
            }
            (adContent as BannerAdContent).HideAd(destroy);
        }

        #endregion

        #region Query methods

        public RuntimePlatform GetActivePlatform() => ApplicationServices.GetActiveOrSimulationPlatform();

        public AdNetworkAdapter GetAdNetworkWithId(string networkId)
        {
            EnsureInitialised();

            return Array.Find(m_activeNetworks, (item) => string.Equals(networkId, item.NetworkId));
        }

        public AdPlacementMeta GetAdPlacementMetaWithPlacement(string placement)
        {
            EnsureInitialised();

            return Array.Find(m_placementMetaArray, (item) => string.Equals(item.Name, placement));
        }

        internal AdNetworkAdapter[] GetPreferredAdNetworks(string placement)
        {
            var     placementMeta   = GetAdPlacementMetaWithPlacement(placement);
            if (placementMeta != null)
            {
                return GetPreferredAdNetworks(placementMeta.AdType);
            }
            return new AdNetworkAdapter[0];
        }

        internal AdNetworkAdapter[] GetPreferredAdNetworks(AdType adType)
        {
            EnsureInitialised();

            if (m_networkPreferenceMap.TryGetValue(adType, out AdNetworkAdapter[] array))
            {
                return array;
            }
            return null;
        }

        public AdNetworkAdapter SelectAdNetwork(AdType adType)
        {
            EnsureInitialised();

            if (m_networkPreferenceMap.TryGetValue(adType, out AdNetworkAdapter[] array))
            {
                return array[0];
            }
            return null;
        }

        private AdContent GetActiveAdContent(string placement)
        {
            EnsureInitialised();

            return m_activeAdContents.Find((item) => string.Equals(placement, item.Placement));
        }

        public string[] GetTestDeviceIds()
        {
            if (m_testDevices == null) return new string[0];

            return m_testDevices.GetDeviceIdsForPlatform(ApplicationServices.GetActiveOrSimulationPlatform());
        }

        #endregion

        #region Getter methods

        public ApplicationPrivacyConfiguration GetPrivacyConfiguration() => m_privacyConfig;

        public User GetUser() => m_user;

        public UserSettings GetUserSettings() => m_userSettings;

        #endregion

        #region Setter methods

        public void SetPlacementMeta(params AdPlacementMeta[] array)
        {
            Assert.IsArgNotNull(array, nameof(array));

            // Cache properties
            m_placementMetaArray    = array;

            // Update internal state
            if (IsActiveAndInitialised() && IsConsentGranted())
            {
                PerformAutoLoadAd();
            }
        }

        public void SetPrivacyConfiguration(ApplicationPrivacyConfiguration config)
        {
            // Cache new value
            m_privacyConfig         = config;

            // Update internal state
            if (IsActiveAndInitialised())
            {
                ExecuteNetworkFunction((network) => network.SetPrivacyConfiguration(config));

                if (IsConsentGranted())
                {
                    PerformAutoLoadAd();
                }
            }
        }

        public void SetUser(User user)
        {
            Assert.IsArgNotNull(user, nameof(user));

            // Cache new value
            m_user  = user;

            // Update internal state
            if (IsActiveAndInitialised())
            {
                ExecuteNetworkFunction((network) => network.SetUser(user));
            }
        }

        public void SetUserSettings(UserSettings settings)
        {
            // Cache new value
            m_userSettings  = settings;

            // Update internal state
            if (IsActiveAndInitialised())
            {
                ExecuteNetworkFunction((network) => network.SetUserSettings(settings));
            }
        }

        public void SetOrientation(ScreenOrientation orientation)
        {
            // Cache new value
            m_orientation   = orientation;

            // Update internal state
            if (IsActiveAndInitialised())
            {
                ExecuteNetworkFunction((network) => network.SetOrientation(orientation));
            }
        }

        #endregion

        #region Private methods

        private bool IsActiveAndInitialised()
        {
            return IsInitialised && gameObject.activeSelf;
        }

        private bool IsConsentGranted()
        {
            return (m_privacyConfig != null) && (ConsentStatus.Authorized == m_privacyConfig.UsageConsent);
        }

        private void RegisterAdNetworks(AdNetworkAdapter[] adNetworks)
        {
            Assert.IsNotNull(adNetworks, nameof(adNetworks));

            // Add object to the subscribed services list
            m_activeNetworks    = adNetworks;

            // Update settings
            UpdatePreferenceMap(preferenceMeta: m_networkPreferenceMeta,
                                preferenceMap: ref m_networkPreferenceMap);
            ExecuteNetworkFunction((adNetwork) =>
                                   {
                                       if (m_privacyConfig != null)
                                       {
                                           adNetwork.SetPrivacyConfiguration(m_privacyConfig);
                                       }
                                       if (m_user != null)
                                       {
                                           adNetwork.SetUser(m_user);
                                       }
                                       if (m_userSettings != null)
                                       {
                                           adNetwork.SetUserSettings(m_userSettings);
                                       }
                                       if (m_orientation != null)
                                       {
                                           adNetwork.SetOrientation(m_orientation.Value);
                                       }
                                   });
        }

        private void UnregisterAdNetworks()
        {
            // Clear instances
            int     activeNetworksCount = (m_activeNetworks == null) ? 0 : m_activeNetworks.Length;
            for (int iter = 0; iter < activeNetworksCount; iter++)
            {
                var     network = m_activeNetworks[iter];
                network.gameObject.SetActive(false);
                Destroy(network.gameObject);
            }

            // Reset property
            m_activeNetworks    = null;
        }

        private void UpdatePreferenceMap(AdNetworkPreferenceMeta preferenceMeta, ref Dictionary<AdType, AdNetworkAdapter[]> preferenceMap)
        {
            m_networkPreferenceMap.Clear();

            // Add new information
            preferenceMap[AdType.Banner]                    = ConvertAdNetworkIdsToObjects(manager: this,
                                                                                  ids: preferenceMeta.Banner);
            preferenceMap[AdType.Interstitial]              = ConvertAdNetworkIdsToObjects(manager: this,
                                                                                  ids: preferenceMeta.Interstitial);
            
            preferenceMap[AdType.RewardedInterstitial]      = ConvertAdNetworkIdsToObjects(manager: this,
                                                                                    ids: preferenceMeta.RewardedInterstitial);
            
            preferenceMap[AdType.Video]            = ConvertAdNetworkIdsToObjects(manager: this,
                                                                                  ids: preferenceMeta.Video);
            preferenceMap[AdType.RewardedVideo]    = ConvertAdNetworkIdsToObjects(manager: this,
                                                                                  ids: preferenceMeta.RewardedVideo);
            //preferenceMap[AdType.AugmentedReality] = ConvertAdNetworkIdsToObjects(manager: this,
            //                                                                      ids: preferenceMeta.AugmentedReality);
            //preferenceMap[AdType.Playable]         = ConvertAdNetworkIdsToObjects(manager: this,
            //                                                                      ids: preferenceMeta.Playable);
            //preferenceMap[AdType.IapPromo]         = ConvertAdNetworkIdsToObjects(manager: this,
            //                                                                      ids: preferenceMeta.IapPromo);
        }

        private void ExecuteNetworkFunction(Action<AdNetworkAdapter> function)
        {
            for (int iter = 0; iter < m_activeNetworks.Length; iter++)
            {
                var     current = m_activeNetworks[iter];
                if (current != null)
                {
                    function(current);
                }
            }
        }

        private void UpdateActiveLoadOperations()
        {
            // This is critical section as multiple callbacks may trigger this in parallel.
            lock (s_objectLock)
            {
                // Added for avoiding errors occuring due to domain reload
                if (m_inprogressLoadOperations == null) return;

                for (int iter = 0; iter < m_inprogressLoadOperations.Count; iter++)
                {
                    var current = m_inprogressLoadOperations[iter];
                    if (!current.IsDone) continue;

                    // Remove finished operations
                    m_inprogressLoadOperations.RemoveAt(iter);
                    iter--;

                    // Move successfully completed ones to the completed list
                    if (current.Status == AsyncOperationStatus.Succeeded)
                    {
                        m_completedLoadOperations.Add(current);
                    }
                }
            }
        }

        private void PerformAutoLoadAd()
        {
            for (int iter = 0; iter < m_placementMetaArray.Length; iter++)
            {
                var     current = m_placementMetaArray[iter];
                if (current.AutoLoad)
                {
                    LoadAd(placement: current.Name,
                           options: current.ContentOptions);
                }
            }
        }

        private bool TryAutoLoadAd(string placement)
        {
            AdPlacementMeta placementMeta;
            if ((placementMeta = Array.Find(m_placementMetaArray, (item) => string.Equals(item.Name, placement) && item.AutoLoad)) != null)
            {
                LoadAd(placement: placementMeta.Name,
                       options: placementMeta.ContentOptions);
                return true;
            }
            return false;
        }

        private IEnumerator TryAutoLoadAdAfterDelay(string placement, int delayInSeconds)
        {
            DebugLogger.Log($"If auto load is enabled for this placement, we will retry auto loading ad after a delay of {delayInSeconds} seconds.");

            yield return new WaitForSeconds(delayInSeconds);
            TryAutoLoadAd(placement);
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            #if UNITY_EDITOR

            for(int iter = 0; iter < m_placementMetaArray.Length; iter++)
            {
                    var placementId = m_placementMetaArray[iter].Name;
                    AdContent adContent = GetActiveAdContent(placementId);
                    if (adContent != null && adContent.IsShowing)
                    {
                        adContent.ShowAd(forcefully: true);
                    }
            }

            #endif
        }

        #endregion

        #region Operation methods

        private LoadAdOperation GetLoadAdOperation(string placement)
        {
            EnsureInitialised();

            // First check whether specified id is already loaded
            var     existingOperation   = FindLoadAdOperationForPlacement(m_completedLoadOperations, placement);
            if (existingOperation == null)
            {
                existingOperation       = FindLoadAdOperationForPlacement(m_inprogressLoadOperations, placement);
            }
            return existingOperation;
        }

        private LoadAdOperation CreateLoadAdOperation(string placement,
                                                      AdContentOptions options)
        {
            EnsureInitialised();

            // Start operation
            LoadAdOperation newOp;
            switch (PreferredLoadAdMode)
            {
                case LoadAdMode.Concurrent:
                    newOp   = new ConcurrentLoadAdOperation(placement: placement,
                                                            manager: this);
                    break;

                case LoadAdMode.Sequential:
                default:
                    newOp   = new SequentialLoadAdOperation(placement: placement,
                                                            manager: this);
                    break;
            }
            newOp.ContentOptions    = options;
            m_inprogressLoadOperations.Add(newOp);
            newOp.OnComplete += (op) => OnLoadAdComplete(op);
            newOp.Start();

            return newOp;
        }

        private LoadAdOperation FindLoadAdOperationForPlacement(List<LoadAdOperation> list, string placement)
        {
            return list.Find((item) => (item.Manager == this) && string.Equals(placement, item.Placement));
        }

        internal LoadAdRequest GetLoadAdRequest(string placement)
        {
            var     loadOp  = GetLoadAdOperation(placement);
            if (loadOp != null)
            {
                return new LoadAdRequest(loadOp);
            }
            return null;
        }

        internal LoadAdRequest CreateLoadAdRequest(string placement,
                                                   AdContentOptions options)
        {
            var     newOp   = CreateLoadAdOperation(placement, options);
            return new LoadAdRequest(newOp);

        }

        #endregion

        #region Internal methods

        internal bool PreprocessAdRequest(out Error error)
        {
            if (!IsActiveAndInitialised())
            {
                error   = AdError.InitializationError("Ad network is not initialised.");
                DebugLogger.LogError(AdsKitSettings.Domain, $"Discarding ad request. Error: {error}.");
                return false;
            }
            if (!IsConsentGranted())
            {
                error   = AdError.ConsentNotAvailable();
                DebugLogger.LogError(AdsKitSettings.Domain, $"Discarding ad request. Error: {error}.");
                return false;
            }

            error   = null;
            return true;
        }

        #endregion

        #region Event handler methods

        private void OnInitialiseComplete(IAsyncOperation<InitialiseResult> sender)
        {
            // Update state properties
            m_initAdNetworksOperation   =  null;
            m_initConsentOperation      = null;
            IsInitialised               = sender.Error == null;
            
            if(sender.Error == null)
            {
                OnInitialisationSuccess(sender.Result);
            }
            else
            {
                OnInitialisationFail(sender.Error);
            }
        }

        private void OnInitialisationSuccess(InitialiseResult result)
        {
            RegisterAdNetworks(result.SubscribedAdNetworks);
            
            if (IsConsentGranted())
            {
                PerformAutoLoadAd();
            }

            SendInitialisationCompleteEvent(result);
        }

        private void OnInitialisationFail(Error error)
        {
            SendInitialisationFailEvent(error);
        }

        private void OnLoadAdComplete(IAsyncOperation<LoadAdResult> sender)
        {
            UpdateActiveLoadOperations();

            var placement = ((LoadAdOperation)sender).Placement;
            if (sender.Error == null)
            {
                SendLoadAdCompleteEvent(placement: placement,
                                    result: sender.Result);
            }
            else
            {
                SendLoadAdFailEvent(placement: placement,
                                error: sender.Error);

                IEnumerator routine = TryAutoLoadAdAfterDelay(placement, m_settings.AutoLoadRetryDelay);
                StopCoroutine(routine);
                StartCoroutine(routine);
            }
        }

        // Similarly make private callbacks for OnAdClick, OnAdStart...

        internal void OnShowAdStart(AdContent sender, ShowAdStateInfo stateInfo)
        {
            // Reset load cache
            RemoveCompletedLoadOperation(stateInfo.Placement);

            SendShowAdStartEvent(placement: sender.Placement);
        }
        private void RemoveCompletedLoadOperation(string placement)
        {
            m_completedLoadOperations.Remove((item) => string.Equals(item.Placement, placement));
        }

        internal void OnShowAdClick(AdContent sender, ShowAdStateInfo stateInfo)
        {
            SendShowAdClickEvent(placement: sender.Placement);
        }

        internal void OnShowAdComplete(AdContent sender, ShowAdStateInfo stateInfo)
        {
            HandleClosedAd(sender);
            SendShowAdCompleteEvent(placement: sender.Placement,
                                    result: stateInfo.Result);
            TryAutoLoadAd(sender.Placement);
        }

        internal void OnShowAdFail(AdContent sender, ShowAdStateInfo stateInfo)
        {
            // Reset load cache
            RemoveCompletedLoadOperation(sender.Placement);
            
            HandleClosedAd(sender);
            SendShowAdFailEvent(placement: sender.Placement,
                                error: stateInfo.Error);
            TryAutoLoadAd(sender.Placement);
        }

        internal void OnAdImpressionRecorded(AdContent sender, AdNetworkAdImpressionInfo impressionInfo)
        {
            SendAdImpressionRecordedEvent(placement: impressionInfo.Placement);
        }

        internal void OnAdPaid(AdContent sender, AdTransaction transaction)
        {
            SendAdPaidEvent(placement: transaction.Placement,
                            transaction: transaction);
        }

        internal void OnAdReward(AdContent sender, AdReward reward)
        {
            SendAdRewardEvent(placement: reward.Placement,
                            reward: reward);
        }

        private void HandleClosedAd(AdContent content)
        {
            // Reset state
            m_activeAdContents.Remove(content);
        }

        #endregion

        #region Report events methods

        private void SendInitialisationCompleteEvent(InitialiseResult result)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending InitialisationComplete event.");

            m_initListeners.SendEvent((item) => item.OnInitialisationComplete(result: result));
        }

        private void SendInitialisationFailEvent(Error error)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending InitialisationFail event. Error: {error}.");

            m_initListeners.SendEvent((item) => item.OnInitialisationFail(error: error));
        }

        private void SendLoadAdCompleteEvent(string placement, LoadAdResult result)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending LoadAdComplete event. Placement: {placement}.");

            m_loadAdListeners.SendEvent((item) => item.OnLoadAdComplete(placement: placement, result: result));
        }

        private void SendLoadAdFailEvent(string placement, Error error)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending LoadAdFail event. Placement: {placement}. Error: {error}.");

            m_loadAdListeners.SendEvent((item) => item.OnLoadAdFail(placement: placement, error: error));
        }

        private void SendShowAdStartEvent(string placement)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending ShowAdStart event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnShowAdStart(placement: placement));
        }

        private void SendShowAdClickEvent(string placement)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending ShowAdClick event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnShowAdClick(placement: placement));
        }

        private void SendShowAdCompleteEvent(string placement, ShowAdResult result)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending ShowAdComplete event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnShowAdComplete(placement: placement, result: result));
        }

        private void SendShowAdFailEvent(string placement, Error error)
        {
            DebugLogger.LogError($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending ShowAdFail event. Placement: {placement}. Error: {error}.");

            m_showAdListeners.SendEvent((item) => item.OnShowAdFail(placement: placement, error: error));
        }

        private void SendAdImpressionRecordedEvent(string placement)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending AdImpressionRecorded event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnAdImpressionRecorded(placement: placement));
        }

        private void SendAdPaidEvent(string placement, AdTransaction transaction)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending AdPaid event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnAdPaid(placement: placement, transaction: transaction));
        }

        private void SendAdRewardEvent(string placement, AdReward reward)
        {
            DebugLogger.Log($"{AdsKitSettings.Domain}:{gameObject.name}", $"Sending AdReward event. Placement: {placement}.");

            m_showAdListeners.SendEvent((item) => item.OnAdReward(placement: placement, reward: reward));
        }


        #endregion
        #region Helper methods

        private void CleanupUsedAds(string placement)
        {
            var adContent = GetActiveAdContent(placement);

            if(adContent != null)
            {
                if ((adContent is BannerAdContent))
                {
                    (adContent as BannerAdContent).HideAd(destroy: true);
                }
            }
        }

        #endregion
    }
}