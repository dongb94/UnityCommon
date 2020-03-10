using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetBundleCompressorGUI : EditorWindow
{
    
    #region <GUI>

    private bool toggle;
    private string targetPath;
    private string compressingPath;
    private string result;

    private bool _onCompressing;

    /// <summary>
    /// 창 설정
    /// </summary>
    [MenuItem("CustomSystem/AssetBundleCompressor")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(AssetBundleCompressorGUI));
        var width = 600;
        var height = 120;
        window.position = new Rect((Screen.width-width)/2 , (Screen.height-height)/2, width, height);
        window.minSize = new Vector2(500,120);
        window.Show();
    }

    private void OnEnable()
    {
        _onCompressing = false;
        targetPath = AssetBundlePath.BundlePath;
        compressingPath = AssetBundlePath.SVNPath;
    }

    /// <summary>
    /// GUI Draw
    /// </summary>
    private async void OnGUI()
    {
        var toggleText = toggle ? "Custom Setting" : "Default Setting";
        toggle = EditorGUILayout.BeginToggleGroup (toggleText, toggle);
        if (toggle)
        {
            targetPath = EditorGUILayout.TextField("Bundle Path", targetPath);
            compressingPath = EditorGUILayout.TextField("SVN Path", targetPath);
        }
        else
        {
            EditorGUILayout.TextField("Bundle Path", AssetBundlePath.BundlePath);
            EditorGUILayout.TextField("SVN Path", AssetBundlePath.SVNPath);
        }
        EditorGUILayout.EndToggleGroup ();

        if (GUILayout.Button("Compress"))
        {
            if (toggle)
            {
                AssetBundlePath.SetBundlePath(targetPath);
                AssetBundlePath.SetSVNPath(compressingPath);
            }
            
            _onCompressing = true;
            result = "압축중";
            result = await Task.Run(AssetBundleCompressor.Instance.MakeAssetBundleZipFile);
            _onCompressing = false;
            AssetBundleCompressor.Instance.Delete();
        }
        var style = new GUIStyle();
        GUIStyleState state = new GUIStyleState();
        state.background = GUI.skin.button.normal.background;
        state.textColor = Color.blue;
        style.active = GUI.skin.button.active;
        style.normal = state;
        style.alignment = TextAnchor.MiddleCenter;
        style.padding = new RectOffset(5,5,3,3);
        style.margin = new RectOffset(160,5, 5,0); 
        style.border = new RectOffset(5,5,1,1);
        style.clipping = TextClipping.Clip;

        if (GUILayout.Button("Show in Explorer", style))
        {
            System.Diagnostics.Process.Start(AssetBundlePath.SVNPath);
        }
        
        EditorGUILayout.LabelField(result??"");

        if(_onCompressing)
            EditorGUILayout.LabelField(AssetBundleCompressor.Instance.progress + "/" + AssetBundleCompressor.Instance.nOfFile);
    }
    #endregion
}