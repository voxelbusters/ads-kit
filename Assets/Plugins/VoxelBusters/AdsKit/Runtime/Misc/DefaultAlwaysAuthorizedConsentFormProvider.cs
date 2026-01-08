using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// The default consent form provider which always has consent status as authorized. This has lowest priority and if you have any custom consent provider to be considered, use a higher priority than this(-1).
    /// \note This is only used when there is no consent form provider implementation available and consider user provided the consent by default.
    /// </summary>
    public class DefaultAlwaysAuthorizedConsentFormProvider : IConsentFormProvider
    {
        public int Priority => -1;

        public bool? IsAgeRestrictedUser { get; set; }

        public void ResetConsentInformation()
        {
            
        }

        public void ShowConsentForm(bool forceShow = false, CompletionCallback<ApplicationPrivacyConfiguration> callback = null)
        {
            var     config  = new ApplicationPrivacyConfiguration(usageConsent: ConsentStatus.Authorized, isAgeRestrictedUser: IsAgeRestrictedUser);
            callback?.Invoke(config, null);
        }
    }
}