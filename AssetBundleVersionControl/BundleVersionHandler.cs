using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using UnityEngine;

public class BundleVersionHandler
{
    private static BundleVersionHandler _instance;

    public float Percentage;

    private const string ZIP_FILE_EXTENSION = ".zip";
    private readonly string bundlePath;
    private const string LocalBundleSubPath = "BundleResources/AssetBundle/Bundle/";
    private const string ServerBundlePath = "Bundle/";
    private const string VersionPath = "version.txt";

    private int _currentVersion;
    private Dictionary<string, int> _localVersion;
    private Dictionary<string, int> _serverVersion;
    private List<string> _downloadList;
    
    private uint falseCount;
    
    private int SelectedServerNum;
    private string[] serverURL =
    {
        "http://211.180.114.22/PuccaAssetBundle/",
        "http://211.180.114.33/PuccaAssetBundle/"
    };

    public static BundleVersionHandler Instance => _instance??new BundleVersionHandler();

    /// <summary>
    /// make to singleton
    /// </summary>
    private BundleVersionHandler()
    {
        _instance = this;
        falseCount = 0;
        Percentage = 0;
        
        _localVersion = new Dictionary<string, int>();
        _serverVersion = new Dictionary<string, int>();
        _downloadList = new List<string>();
        
        bundlePath = String.Format($"{Application.persistentDataPath}/");
        
        //select download server
        SelectedServerNum = UnityEngine.Random.Range(0,serverURL.Length);
        
        LoadVersionFile();
        
    }
    
    /// <summary>
    /// 로컬 에셋번들 버전과 서버의 에셋번들 버전을 비교한다.
    /// </summary>
    /// <returns></returns>
    public bool CheckAssetBundleVersion()
    {
        int result;
        do
        {
            result = RequestAssetBundleVersion();
        } while (result == -1);
        
        return _currentVersion == result;
    }

    /// <summary>
    /// 서버에 있는 최신버전 에셋 번들을 다운로드 받는다.
    /// </summary>
    public int DownloadAssetBundle()
    {
        UpdateDownloadList();

        for (int i = 0; i < _downloadList.Count; i++)
        {
            var zipFilePath = _downloadList[i] + ZIP_FILE_EXTENSION;
            var subDirectoryPath = "";
            if(zipFilePath.LastIndexOf('/')!=-1)
                subDirectoryPath = zipFilePath.Substring(0, zipFilePath.LastIndexOf('/'));
            
            var request = (HttpWebRequest)WebRequest.Create(serverURL[SelectedServerNum] + ServerBundlePath + zipFilePath);
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 30 * 1000;
            //request.Headers.Add("Authorization", "BASIC SGVsbG8=");

            Debug.Log(serverURL[SelectedServerNum] + _downloadList[i] + ZIP_FILE_EXTENSION);
            
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    HttpStatusCode statusCode = response.StatusCode;
            
                    var buffer = new byte[1024];
                    var pos = 0;
                    var count = 0;
            
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        var size = responseStream.Length; // 에셋번들 총길이
                        var downloadSize = 0;
                        
                        DirectoryExistCheck(bundlePath + LocalBundleSubPath + subDirectoryPath);
                        using (var fileStream = File.Create(bundlePath + LocalBundleSubPath + zipFilePath))
                        {
                            do
                            {
                                count = responseStream.Read(buffer, pos, buffer.Length);
                                fileStream.Write(buffer, 0, count);
                                downloadSize += count;
                                Percentage = (float)downloadSize / size;
                            } while (count > 0);
                        }
                    }            
                }
            }
            catch (WebException e)
            {
                Debug.LogError(e.Message);
                Server404();
                return -1;
            }
        }
        
        falseCount = 0;
        return 0;
    }

    /// <summary>
    /// 로컬의 버전파일을 서버와 동기화한다.
    /// </summary>
    public bool UpdateVersionInfo()
    {
        var request = (HttpWebRequest)WebRequest.Create(serverURL[SelectedServerNum] + VersionPath);
        request.Method = WebRequestMethods.Http.Get;
        request.Timeout = 30 * 1000;
        //request.Headers.Add("Authorization", "BASIC SGVsbG8="); 

        try
        {
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                HttpStatusCode statusCode = response.StatusCode;
                Debug.Log(statusCode); // success msg = "OK"
            
                var buffer = new byte[1024];
                var pos = 0;
                var count = 0;

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var fileStream = File.Create(bundlePath + VersionPath))
                    {
                        do
                        {
                            count = responseStream.Read(buffer, pos, buffer.Length);
                            fileStream.Write(buffer, 0, count);
                        } while (count > 0);
                    }
                }
            }
        }
        catch (WebException e)
        {
            Debug.LogError(e.Message);
            Server404();
            return false;
        }
        
        falseCount = 0;
        return true;
    }

    /// <summary>
    /// 에셋 번들 압축파일을 로컬 폴더에 푼다.
    /// </summary>
    public void UnpackAssetBundle()
    {
        var basePath = bundlePath + LocalBundleSubPath;

        for (int i = 0; i < _downloadList.Count; i++)
        {
            var zipPath = basePath + _downloadList[i] + ZIP_FILE_EXTENSION;
            var directoryPath = zipPath.Substring(0, zipPath.LastIndexOf('/'));

            DirectoryExistCheck(directoryPath);
            
            using (var source = ZipFile.Open(zipPath, ZipArchiveMode.Read, null))
            {
                for (var index = 0; index < source.Entries.Count; index++)
                {
                    ZipArchiveEntry entry = source.Entries[index];
                    string fullPath = Path.GetFullPath(Path.Combine(directoryPath, entry.FullName));

                    if (Path.GetFileName(fullPath).Length == 0)
                    {
                        if (entry.Length != 0L)
                            throw new IOException("IO_DirectoryNameWithData");
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        FileMode mode = FileMode.Create;
                        using (var destination = (Stream) File.Open(fullPath, mode, FileAccess.Write, FileShare.None))
                        {
                            using (var stream = entry.Open())
                                stream.CopyTo(destination);
                        }
                    }
                }
            }
            File.Delete(zipPath);
        }
    }

    public void DeleteInstance()
    {
        _instance = null;
    }

    /// <summary>
    /// 버젼 텍스트 파일 정보를 읽어온다.
    /// </summary>
    private void LoadVersionFile()
    {
        if (!File.Exists(bundlePath + VersionPath))
        {
            _currentVersion = 0;
            return;
        }
        using (var file = File.OpenRead(bundlePath + VersionPath))
        {
            var buffer = new byte[65535];
            var count = file.Read(buffer, 0, buffer.Length);
            var versionFile = Encoding.UTF8.GetString(buffer).Substring(0,count).Split('\n');
            _currentVersion = int.Parse(versionFile[0]);
            for (int i = 1; i < versionFile.Length; i++)
            {
                if (versionFile[i] == "" || versionFile[i] == null) continue;
                var versionInfo = versionFile[i].Split(':');
                _localVersion.Add(versionInfo[0], int.Parse(versionInfo[1]));
            }
        }
    }
    
    /// <summary>
    /// 서버에 저장된 에셋 번들의 버전정보를 저장하고 버전 번호를 반환한다.
    /// </summary>
    /// <returns>성공시 에셋번들의 최신버전, 실패시 -1</returns>
    private int RequestAssetBundleVersion()
    {
        string file;
        
        var request = (HttpWebRequest)WebRequest.Create(serverURL[SelectedServerNum] + VersionPath);
        request.Method = WebRequestMethods.Http.Get;
        request.Timeout = 30 * 1000;
        //request.Headers.Add("Authorization", "BASIC SGVsbG8=");


        try
        {
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                HttpStatusCode statusCode = response.StatusCode;
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        file = streamReader.ReadToEnd();
                    }
                }
            }
        }
        catch (WebException e)
        {
            Debug.LogError(e.Message);
            Server404();
            return -1;
        }

        falseCount = 0;

        Debug.Log("URL : "+serverURL[SelectedServerNum] + VersionPath);
        Debug.Log("File Length : " + file.Length);
        Debug.Log("File String : " + file);

        var versionFile = file.Split('\n');
        
        for (int i = 1; i < versionFile.Length; i++)
        {
            if (versionFile[i] == "" || versionFile[i] == null) continue;
            var versionInfo = versionFile[i].Split(':');
            _serverVersion.Add(versionInfo[0], int.Parse(versionInfo[1]));
        }
        
        return int.Parse(versionFile[0]);
    }

    /// <summary>
    /// 서버의 버전정보와 로컬파일의 버전정보를 비교해 다운로드 받을 파일의 목록을 갱신한다.
    /// </summary>
    private void UpdateDownloadList()
    {
        foreach (var bundleVersion in _serverVersion)
        {
            var isChanged = true;
            if(_localVersion.ContainsKey(bundleVersion.Key))
            {
                isChanged = _localVersion[bundleVersion.Key] != bundleVersion.Value;
            }

            Debug.Log($"{bundleVersion.Key} - {bundleVersion.Value} isChanged : {isChanged}");
            
            if(isChanged) _downloadList.Add(bundleVersion.Key);
        }
    }

    private void DirectoryExistCheck(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
    
    private void Server404()
    {
        SelectedServerNum = (SelectedServerNum + 1) % serverURL.Length;
        falseCount += 1;
        if (falseCount >= (uint)serverURL.Length)
        {
            Debug.LogError("No accessible asset bundle download server");
#if !UNITY_EDITOR
            NoticeUIManager.PopUpTextMessage(NoticeUIManager.Title.Confirm, BootSceneLanguageData.GetContent(10000), Application.Quit);
#endif
            throw new UnityException("Quit PlayMode");
        }
    }
}