using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace Fulbo.Admob
{
    public class AdsManager : MonoBehaviour
    {
        const string SDK = "L7vO47bBhwCQmKqSKdCzZeSm27sgKRQQyJQbT7qVtrFyZIzKZ4l_4KJ2sT1_hs70gCVuEBEDbaoxzxPBeQRG1u";

        string rewardedAdUnitId = "a27bce0b3dc74635";
        string interstitialAdUnitId = "6b2cd31ef1cb3be1";

        int retryAttempt;
        
        public void Start()
        {

//#if UNITY_IOS
//        rewardedAdUnitId = "5d7209eed1474617";
//#endif
//            if (DB.DBManager.Instance.versionMode == DB.DBManager.versionModes.PROD)
//            {
//                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
//                {
//                    InitializeRewardedAds();
//                    InitializeIntersititialAds();
//                };
//            }
//            else
//            {
//                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
//                {
//                    // AppLovin SDK is initialized, start loading ads
//                    InitializeRewardedAds();
//                    InitializeIntersititialAds();
//                    MaxSdk.ShowMediationDebugger();
//                };
//            }

//            MaxSdk.SetSdkKey(SDK);
//            //MaxSdk.SetUserId(USER_ID);
//            MaxSdk.InitializeSdk();

            Events.AdsWatchVideo += AdsWatchVideo;
            Events.AdsWatchInterstitial += AdsWatchInterstitial;
        }
        public void OnDestroy()
        {
            Events.AdsWatchVideo -= AdsWatchVideo;
            Events.AdsWatchInterstitial -= AdsWatchInterstitial;
        }
       
        System.Action<bool> OnReady;
        void AdsWatchVideo(System.Action<bool> OnReady)
        {
            this.OnReady = OnReady;
            ShowRewardedAd();
        }
        void AdsWatchInterstitial(System.Action<bool> OnReady)
        {
            this.OnReady = OnReady;
            ShowInterstitialAd();
        }
        private void ShowRewardedAd()
        {
            //if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
            //{
            //    Debug.Log("ADS AdsWatchVideo");
            //    MaxSdk.ShowRewardedAd(rewardedAdUnitId);
            //} else {
            //    Events.OnPopup(Data.Instance.texts.Get("no_available_ads"), () => OnReady(false));
            //}
        }
        private void ShowInterstitialAd()
        {
            //if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
            //{
            //    Debug.Log("ADS Interstitial");
            //    MaxSdk.ShowInterstitial(interstitialAdUnitId);
            //}
        }

        public void InitializeIntersititialAds()
        {
            // Attach callback
            //MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            //MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            //MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            //MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            //MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            //MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

            // Load the first rewarded ad
            LoadInterstitial();
        }

        public void InitializeRewardedAds()
        {
            // Attach callback
            //MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            //MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            //MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            //MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            //MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            //MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            //MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            //MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
        //    MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }
        private void LoadInterstitial()
        {
         //   MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        //private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    Debug.Log(" Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.");

        //    // Reset retry attempt
        //    retryAttempt = 0;
        //}

        //private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        //{
        //    Debug.Log("OnRewardedAdLoadFailedEvent retryAttempt: " + retryAttempt);

        //    // Rewarded ad failed to load 
        //    // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        //    retryAttempt++;

        //    if (retryAttempt > 6) {
        //        if (OnReady != null)
        //            OnReady(false);
        //    } else {
        //        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        //        Invoke("LoadRewardedAd", (float)retryDelay);
        //    }
        //}

        //private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {

        //    Debug.Log("OnRewardedAdDisplayedEvent retryAttempt: " + adInfo);

        //}

        //private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        //{
        //    Debug.Log("LoadRewardedAd adUnitId: " + adUnitId + " errorInfo: " + errorInfo.Message);
        //    // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        //    LoadRewardedAd();
        //}

        //private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {

        //    Debug.Log("OnRewardedAdClickedEvent: " + adInfo);
        //}

        //private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    // Rewarded ad is hidden. Pre-load the next ad
        //    LoadRewardedAd();
        //}
        //private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        //{
        //    Debug.Log("OnRewardedAd Received Reward Event adUnitId: " + adUnitId + " Label: " + reward.Label + " Amount:" + reward.Amount);
        //    OnReady(true);

        //    //Analytics
        //    //Dictionary<string, object> param = new Dictionary<string, object>();
        //    //param["adType"] = reward.Label;
        //    //param["adID"] = adUnitId;

        //    //Events.OnTrack("OnAdDone", param);

        //    // The rewarded ad displayed and the user should receive the reward.
        //}

        //private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    Debug.Log("Load RewardedAd RevenuePaidEvent: " + adUnitId + " adInfo: " + adInfo);
        //    //OnReady(true);
        //    // Ad revenue paid. Use this callback to track user revenue.
        //}










        //private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        //    // Reset retry attempt
        //    retryAttempt = 0;
        //}

        //private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        //{
        //    // Interstitial ad failed to load 
        //    // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        //    retryAttempt++;
        //    double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        //    Invoke("LoadInterstitial", (float)retryDelay);
        //}

        //private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        //    OnReady(true);
        //}

        //private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        //{
        //    // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        //    LoadInterstitial();
        //}

        //private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        //private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        //{
        //    // Interstitial ad is hidden. Pre-load the next ad.
        //    LoadInterstitial();
        //}

    }

}