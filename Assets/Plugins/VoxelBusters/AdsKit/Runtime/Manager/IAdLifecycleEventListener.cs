using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    /// <summary>
    /// Implement this interface to handle Initialize results.
    /// </summary>
    [IncludeInDocs]
    public interface IInitialiseEventListener : IEventHandler
    {
        /// <summary>
        /// This callback method handles logic for the SDK successfully initializing.
        /// </summary>
        /// <param name="result">The result.</param>
        void OnInitialisationComplete(InitialiseResult result);

        /// <summary>
        /// This callback method handles logic for the SDK failing to initialize.
        /// </summary>
        /// <param name="error">The error that caused initialization to fail.</param>
        void OnInitialisationFail(Error error);
    }

    /// <summary>
    /// Implement this interface to handle Load results.
    /// </summary>
    [IncludeInDocs]
    public interface ILoadAdEventListener : IEventHandler
    {
        /// <summary>
        /// This callback method handles logic for successfully loading the ad.
        /// </summary>
        /// <param name="placement">The identifier for the ad-unit/placement that loaded content.</param>
        /// <param name="result">The result.</param>
        void OnLoadAdComplete(string placement, LoadAdResult result);

        /// <summary>
        /// This callback method handles logic for failing to load the ad.
        /// </summary>
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        /// <param name="error">The error that caused load to fail.</param>
        void OnLoadAdFail(string placement, Error error);
    }

    /// <summary>
    /// Implement this interface to handle Show results.
    /// </summary>
    [IncludeInDocs]
    public interface IShowAdEventListener : IEventHandler
    {
        /// <summary>
        /// This callback method handles logic for the ad starting to play.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        void OnShowAdStart(string placement);

        /// <summary>
        /// This callback method handles logic for the user clicking on the ad.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        void OnShowAdClick(string placement);

        /// <summary>
        /// This callback method handles logic for the ad finishing.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        void OnShowAdComplete(string placement, ShowAdResult result);

        /// <summary>
        /// This callback method handles logic for failing to show the ad.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        /// <param name="error">The error that caused load to show.</param>
        void OnShowAdFail(string placement, Error error);

        /// <summary>
        /// This callback method handles logic for the recording of ad impression.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        void OnAdImpressionRecorded(string placement);

        /// <summary>
        /// This callback method handles logic for the ad transaction.
        /// <param name="placement">The identifier for the ad-unit/placement.</param>
        /// <param name="transaction">The transaction.</param>
        void OnAdPaid(string placement, AdTransaction transaction);

        /// <summary>
        /// This callback gets called when there is a reward for the ad.
        /// </summary>
        /// <param name="placement"></param>
        /// <param name="rewardInfo"></param>
        void OnAdReward(string placement, AdReward reward);
    }

    /// <summary>
    /// Implement this interface to handle various states of an ad. 
    /// </summary>
    [IncludeInDocs]
    public interface IAdLifecycleEventListener : IInitialiseEventListener, ILoadAdEventListener, IShowAdEventListener
    { }
}