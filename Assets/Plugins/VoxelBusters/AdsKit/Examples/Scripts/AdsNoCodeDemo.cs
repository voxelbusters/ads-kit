using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;


namespace VoxelBusters.AdsKit.Demo
{
    public class AdsNoCodeDemo : MonoBehaviour
    {
        
         public void OnAdsInitialisation()
        {
            Debug.Log($"[AdsNoCodeDemo] Successfully initialised AdsKit.");
        }

        public void OnAdsInitialisationError(Error error)
        {
            Debug.LogError($"[AdsNoCodeDemo] Received error: {error} when initialising AdsKit.");
        }

        public void OnAdLoad(string adPlacement)
        {
            Debug.Log($"[AdsNoCodeDemo] Successfully loaded ad for placement id: {adPlacement}");
        }

        public void OnAdLoadFailed(string adPlacement, Error error)
        {
            Debug.LogError($"[AdsNoCodeDemo] Received error: {error} when loading ad with placement id:{adPlacement}");
        }


        public void OnAdShow(string adPlacement)
        {
            Debug.Log($"[AdsNoCodeDemo] Successfully showed ad for placement id: {adPlacement}");
        }


        public void OnAdFinish(string adPlacement)
        {
            Debug.Log($"[AdsNoCodeDemo] Successfully finished ad for placement id: {adPlacement}");
        }


        public void OnAdReward(string placement, AdReward reward)
        {
            if(reward.IsRewardAmountAvailable())
            {
                Debug.Log($"[AdsNoCodeDemo] AdReward: {reward} received for placement id: {placement}");
            }
            else
            {
                Debug.Log($"[AdsNoCodeDemo] Eligible for rewarding the user for placement id: {placement} with your own defined amount.");
            }
            
        }

        public void OnAdShowFailed(string adPlacement, Error error)
        {
            Debug.LogError($"[AdsNoCodeDemo] Received error: {error} when showing ad with placement id:{adPlacement}");
        }


        public void OnAdLoadComplete(string adPlacement, LoadAdResult result)
        {
            Debug.Log($"[AdsNoCodeDemo] Successfully loaded ad for placement id: {adPlacement} with result: {result}");
        }
    }
}
