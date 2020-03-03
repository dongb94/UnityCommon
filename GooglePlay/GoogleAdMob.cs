/* 유니티 구글 ADMOB 메뉴얼
* https://developers.google.com/admob/unity/start?hl=ko
*/
using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

/// <summary>
/// 구글 광고를 다루는 클레스
/// </summary>
public class GoogleAdMob
{
    public bool isTest; // 테스트 플레그 (true시 실재 광고 안나옴)
    public Text textOutput; // 베너광고용 테스트 텍스트
    
    private static GoogleAdMob instance;

    private const string InterstitialAdID = "ca-app-pub-****************/**********";// 전면 광고 ID
    private const string BannerAdID = "ca-app-pub-****************/**********"; // 베너 광고 ID
    private const string RewardedAdID = "ca-app-pub-****************/**********"; // 보상형 광고 ID

    private InterstitialAd _interstitialAd; // 전면 광고 객체
    private BannerView _bannerView; // 베너 광고 객체
    private RewardedAd _rewardedAd; // 보상형 광고 객체
    private bool _isOnPlayBannerView;
    private bool _isBannerViewLoaded;

    public int bannerX=170, bannerY=280; // 베너광고 위치
    public float ScreenSize; // 스크린 배율
    
    private string _unitID;

    public static GoogleAdMob Instance
    {
        get
        {
            if (instance == null)
            {
                new GoogleAdMob();
            }
            return instance;
        }
    }
    
    private string AppId  
    {
        get
        {
            if (isTest)
            {
#if UNITY_ANDROID
                return "ca-app-pub-3940256099942544~3347511713"; // 안드로이드 테스트 앱 ID
#elif UNITY_IPHONE
                return "ca-app-pub-3940256099942544~1458002511"; // mac os 테스트 앱 ID
#else
                return "unexpected_platform";
#endif                
            }
            else
            {
                return "ca-app-pub-3299070289462023~8946188559"; // 앱의 ID
            }
        }
    }

    private GoogleAdMob()
    {
        ///// 서비스시 주석처리 //////
        isTest = true;
        ////////////////////////////
        
        instance = this;
        MobileAds.Initialize(AppId);
        LoadRewardedAd();
        LoadBannerAd();
        ScreenSize = MobileAds.Utils.GetDeviceScale();
        if (ScreenSize < 0.1) ScreenSize = 1;
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
            return;
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = InterstitialAdID;
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
            return;
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = BannerAdID;
        }

        // 광고 크기 설정
        var width = (Screen.width * 0.625)/MobileAds.Utils.GetDeviceScale();
        var height = (Screen.height * 0.12)/MobileAds.Utils.GetDeviceScale();
        
        //_bannerView = new BannerView(_unitID, AdSize.Banner, AdPosition.Bottom); // 기본으로 제공하는 값을 통해 위치 설정
        _bannerView = new BannerView(_unitID, AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth((int)width), bannerX, bannerY); // 맞춤 설정 (광고 사이즈 및 위치)

        // 로드 실패시의 이벤트 추가
        _bannerView.OnAdFailedToLoad += (sender, args) =>
        {
#if UNITY_EDITOR
            Debug.Log("Banner AD is not Loaded");
#endif
        };
        _bannerView.OnAdLoaded += (sender, args) =>
        {
            _isBannerViewLoaded = true;
            if (_isOnPlayBannerView)
                _bannerView.Show();
            else
                _bannerView.Hide();
        }; 
        _bannerView.Hide(); // 기본 = 숨김상태

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
            return;
#endif
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = RewardedAdID;
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
//        if(!_isBannerViewLoaded)
//            LoadBannerAd();
        _isOnPlayBannerView = true;
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

    private void OnInterstitialAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnInterstitialAdClosed event received.");
        // 전면 광고를 닫을 때 메모리 절약을 위해 객체를 제거한다.
        _interstitialAd.Destroy();
    }

    public void OnRemoveBannerAdScene()
    {
        _isOnPlayBannerView = false;
        
        // 베너광고를 제거하지 않고 숨긴다.
        if (!_isBannerViewLoaded) return;
            _bannerView.Hide();
        
//        // 베너 광고가 없는 화면으로 넘어 갈 때 호출해 메모리 절약을 위해 객체를 제거한다.
//        if (!_isBannerViewLoaded) return;
//        _isBannerViewLoaded = false;
//        _bannerView.Destroy();
    }

    private void OnRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnRewardedAdClosed event received.");
        // 보상형 광고는 1회용 객체로 한번 표시된 후에 파괴된다. 다른 보상형 광고를 보기 위해서는 새로 로드 해야한다.
    }

    /// <summary>
    /// 화면의 위치를 기반으로 베너광고의 위치를 설명한다. (pixel 단위)
    /// </summary>
    public void SetBannerPosition(int x, int y)
    {
        bannerX = (int)(x / ScreenSize);
        bannerY = (int)(y / ScreenSize);
    }

}
