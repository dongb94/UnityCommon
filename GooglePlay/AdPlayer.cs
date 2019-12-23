/* 유니티 구글 ADMOB 메뉴얼
* https://developers.google.com/admob/unity/start?hl=ko
*/
using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdPlayer : MonoBehaviour
{
    public bool isTest;

    private const string AdID = "ca-app-pub-****************/**********"; // 앱에 개제하려는 광고단위의 ID

    private InterstitialAd _interstitialAd; // 전면 광고 객체
    private BannerView _bannerView; // 베너 광고 객체
    private RewardedAd _rewardedAd; // 보상형 광고 객체
    
    private string _unitID;

    private string AppId  
    {
        get
        {
            if (isTest)
            {
#if UNITY_ANDROID
                return "ca-app-pub-3940256099942544~3347511713"; // 테스트 앱 ID
#elif UNITY_IPHONE
            return "ca-app-pub-3940256099942544~1458002511"; // 테스트 앱 ID
#else
            return "unexpected_platform";
#endif                
            }
            else
            {
                return "ca-app-pub-****************~**********"; // 앱의 ID
            }
        }
    }
    
    private void Awake()
    {
        MobileAds.Initialize(AppId);
    }

    /// <summary>
    /// 전면 광고 요청
    /// </summary>
    private void LoadInterstitialAd()
    {
        AdRequest request;
 
        if (isTest)
        {
            request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice("Test")
                .Build();
#if UNITY_ANDROID
            _unitID = "ca-app-pub-3940256099942544/1033173712"; // Test ID
#elif UNITY_IPHONE
            _unitID = "ca-app-pub-3940256099942544/4411468910"; // Test ID
#else
            _unitID = "unexpected_platform";
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = AdID;
        }
        
        // 전면광고 객체 생성
        _interstitialAd = new InterstitialAd(_unitID);
        // 전면광고를 닫을 때의 이벤트 추가
        _interstitialAd.OnAdClosed += OnInterstitialAdClosed;
        // 전면광고 로드
        _interstitialAd.LoadAd(request);
        
#if UNITY_EDITOR
        if(_interstitialAd.IsLoaded()) Debug.Log("Interstitial AD is Loaded");
        else Debug.Log("Interstitial AD is not Loaded");
#endif
    }

    /// <summary>
    /// 베너 광고 요청
    /// </summary>
    private void LoadBannerAd()
    {
        AdRequest request;
 
        if (isTest)
        {
            request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice("Test")
                .Build();
#if UNITY_ANDROID
            _unitID = "ca-app-pub-3940256099942544/6300978111"; // Test ID
#elif UNITY_IPHONE
            _unitID = "ca-app-pub-3940256099942544/2934735716"; // Test ID
#else
            _unitID = "unexpected_platform";
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = AdID;
        }
        
        // BannerView 객체 생성
        _bannerView = new BannerView(_unitID, AdSize.Banner, AdPosition.Top); // 기본 값 사용
        // _bannerView = new BannerView(_unitID, new AdSize(250,250), 0, 50); // 맞춤 설정 (광고 사이즈 및 위치)
        
        // 로드 실패시의 이벤트 추가
        _bannerView.OnAdFailedToLoad += (sender, args) =>
        {
#if UNITY_EDITOR
            Debug.Log("Banner AD is not Loaded");
#endif
        };

        // 베너 광고 로드
        _bannerView.LoadAd(request);
    }

    /// <summary>
    /// 보상형 광고 요청
    /// </summary>
    private void LoadRewardedAd()
    {
        AdRequest request;
 
        if (isTest)
        {
            request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice("Test")
                .Build();
#if UNITY_ANDROID
            _unitID = "ca-app-pub-3940256099942544/5224354917"; // Test ID
#elif UNITY_IPHONE
            _unitID = "ca-app-pub-3940256099942544/1712485313"; // Test ID
#else
            _unitID = "unexpected_platform";
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = AdID;
        }
        
        // 보상형광고 객체 생성
        _rewardedAd = new RewardedAd(_unitID);
        // 보상형광고를 닫을 때의 이벤트 추가
        _rewardedAd.OnAdClosed += OnRewardedAdClosed;
        // 보상형광고 로드
        _rewardedAd.LoadAd(request);
        
#if UNITY_EDITOR
        if(_rewardedAd.IsLoaded()) Debug.Log("RewardedAd AD is Loaded");
        else Debug.Log("RewardedAd AD is not Loaded");
#endif
    }

    public void PlayInterstitialAd()
    {
        if (!_interstitialAd.IsLoaded())
        {
            Debug.Log("Load Interstitial Ad.");
            LoadInterstitialAd();
        }
        _interstitialAd.Show();
    }
    
    public void PlayBannerAd()
    {
        LoadBannerAd();
        _bannerView.Show();
    }

    public void PlayRewardedAd()
    {
        if (!_rewardedAd.IsLoaded())
        {
            Debug.Log("Load Rewarded Ad.");
            LoadRewardedAd();
        }
        _rewardedAd.Show();
    }
    
    public void OnInterstitialAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnInterstitialAdClosed event received.");
        // 전면 광고를 닫을 때 메모리 절약을 위해 객체를 제거한다.
        _interstitialAd.Destroy();
    }

    public void OnMoveOtherScene()
    {
        // 베너 광고가 없는 화면으로 넘어 갈 때 호출해 메모리 절약을 위해 객체를 제거한다.
        _bannerView.Destroy();
    }
    
    public void OnRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnRewardedAdClosed event received.");
        // 보상형 광고는 1회용 객체로 한번 표시된 후에 파괴된다. 다른 보상형 광고를 보기 위해서는 새로 로드 해야한다.
    }

}
