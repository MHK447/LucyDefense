using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using System;

public class AdManager : MonoBehaviour
{
    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif

    private bool IsInterAdLoaded = false;

    private string InitadUnitId = "ca-app-pub-3940256099942544/1033173712"; // 테스트 전면 광고 단위 ID


    void Start()
    {
        MobileAds.Initialize(initStatus => {
            LoadRewardedAd();
            LoadInterstitialAd();
        });
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                IsInterAdLoaded = true;

                _interstitialAd.OnAdFullScreenContentClosed += HandleInterstitialAdClosed;
            });
    }

    // 전면 광고 표시
    public void ShowInterstitialAd()
    {
        if (IsInterAdLoaded && _interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
            Debug.Log("Interstitial ad is being shown.");
            IsInterAdLoaded = false; // 광고 표시 후 다시 로드 필요
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet.");
        }
    }

    public void HandleInterstitialAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        // 광고가 닫힌 후 다시 로드
        LoadInterstitialAd();
    }

    // 리워드 광고 로드
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                _rewardedAd = ad;
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                // 광고가 닫혔을 때 이벤트 핸들러
                _rewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
            });
    }
    // 리워드 광고 표시
    public void ShowRewardedAd(System.Action rewardaction)
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                // 보상 처리를 위한 콜백 호출
                rewardaction?.Invoke();

                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    // 사용자가 보상을 받을 때
    public void HandleUserEarnedReward(Reward reward)
    {
        Debug.Log("User earned reward: " + reward.Amount);
        // 여기에서 보상 지급 로직을 구현하세요
    }

    // 광고가 닫혔을 때
    public void HandleRewardedAdClosed()
    {
        Debug.Log("Rewarded ad closed.");
        // 광고가 닫히면 새로운 광고 로드
        LoadRewardedAd();
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
}