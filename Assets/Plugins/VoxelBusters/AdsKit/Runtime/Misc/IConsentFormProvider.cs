using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [IncludeInDocs]
    public interface IConsentFormProvider
    {
        #region Properties

        int Priority { get; }

        bool? IsAgeRestrictedUser { get; set; }

        #endregion

        #region Public methods

        void ShowConsentForm(bool forceShow = false, CompletionCallback<ApplicationPrivacyConfiguration> callback = null);

        void ResetConsentInformation();

        #endregion
    }
}