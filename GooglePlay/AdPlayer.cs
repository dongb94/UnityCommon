/* 유니티 구글 ADMOB 메뉴얼
* https://developers.google.com/admob/unity/start?hl=ko
*/

using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdPlayer : MonoBehaviour
{
    private const string AdID = "ca-app-pub-****************/**********";

    private InterstitialAd ad;

    public bool isTest;

    private string _unitID;
    
    private void Awake()
    {
        // MobileAds.Initialize(AppID);
        LoadAd();
    }

    private void LoadAd()
    {
        AdRequest request;
 
        if (isTest)
        {
            request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice("Test")
                .Build();
            _unitID = "ca-app-pub-3940256099942544/1033173712"; // Test ID
        }
        else
        {
            request = new AdRequest.Builder().Build();
            _unitID = AdID;
        }
        
        ad = new InterstitialAd(_unitID);
        ad.LoadAd(request);
        
        ad.OnAdClosed += OnInterstitialAdClosed;
        
        #if UNITY_EDITOR
        if(ad.IsLoaded()) Debug.Log("AD is Loaded");
        else Debug.Log("AD is not Loaded");
        #endif
    }

    public void PlayAd()
    {
        if (!ad.IsLoaded())
        {
            Debug.Log("Load Ad.");
            LoadAd();
        }
        ad.Show();
    }
    
    public void OnInterstitialAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleOnInterstitialAdClosed event received.");
 
        ad.Destroy();
        
    }

}
