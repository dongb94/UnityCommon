
using UnityEngine;

/// <summary>
/// 에셋 번들 관리에 필요한 경로를 모아 놓은 클레스
/// </summary>
public static class AssetBundlePath
{
    public const string MANIFEST_FILE_EXTENSION = ".manifest";
    public const string ZIP_FILE_EXTENSION = ".zip";
    
    private static string _bundlePath;
    public static string BundlePath
    {
        get
        {
            if (string.IsNullOrEmpty(_bundlePath))
                _bundlePath = $"{Application.dataPath}/../BundleResources/AssetBundle/Bundle/";
            return _bundlePath;
        }
    }

    private static string _svnPath;
    public static string SVNPath
    {
        get
        {
            if(string.IsNullOrEmpty(_svnPath))
                _svnPath = $"{Application.dataPath}/../../SVN/Bundle/";
            return _svnPath;
        }
    }

    public static string VersionTextPath => SVNPath + "../version.txt";
    public static string VersionDirectoryPath => SVNPath + "../version/";

    public static void SetBundlePath(string path)
    {
        _bundlePath = path;
    }

    public static void SetSVNPath(string path)
    {
        _svnPath = path;
    }
}