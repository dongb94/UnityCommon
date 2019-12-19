
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class Login : MonoBehaviour
{
    private void Awake()
    {
#if PLATFORM_ANDROID
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        //PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        CubeRotate.Init = true;
#endif
        AutoLogin();
    }

    private void Update()
    {
        // Quit Application
        if (Application.platform == RuntimePlatform.Android)
        {	
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }  
    }

    public void AutoLogin()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success =>
            {
                // handle code when success or not success
                if (success) CubeRotate.Rotate = true;
                
                if (!success) CubeRotate.Rotate = false;
            });
        }
    }

    public void Logout()
    {
        PlayGamesPlatform.Instance.SignOut();
        CubeRotate.Rotate = false;
    }

    public void ViewLeaderBoard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }

    public void ShowAchievement()
    {
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }
}
