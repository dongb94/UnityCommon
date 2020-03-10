using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// 에셋번들 버젼 관리를 위한 클레스
/// 파일의 이름과 버전정보를 가진 텍스트 파일과
/// 파일의 해싱 데이터로 구성된다.
/// 클라이언트는 텍스트 파일만 가지고 있어도 업데이트 할 번들의 목록을 확인 할 수 있다.
/// </summary>
public class AssetBundleVersionManager
{
    private static AssetBundleVersionManager _instance;

    private int _currentVersion;
    private readonly Dictionary<string, int> _version;

    public static AssetBundleVersionManager instance => _instance??new AssetBundleVersionManager();

    private AssetBundleVersionManager()
    {
        _instance = this;
        _version = new Dictionary<string, int>();
        LoadVersionFile();
    }

    public void Delete()
    {
        _instance = null;
    }
    
    /// <summary>
    /// 각 압축 파일에 변경 사항이 있을 때 처리하는 함수
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="file"></param>
    public void CheckVersionUp(string filePath, string file)
    {
        var buffer = new byte[20];
        var newFile = Hashing(file); // 새로운 파일의 해시파일
        var versionFilePath = AssetBundlePath.VersionDirectoryPath + filePath;

        // 최초로 생성된 파일일 때
        if (!File.Exists(versionFilePath))
        {
            SaveVersionFile(versionFilePath, newFile);
            _version.Add(filePath, _currentVersion+1);
            return;
        }
            
        using (var curFile = File.OpenRead(versionFilePath))
        {
            curFile.Read(buffer,0,20);
        }
        bool equals = buffer.Length == newFile.Length;

        if(equals)
            for (int i = 0; i < newFile.Length; i++)
            {
                if (buffer[i] != newFile[i]) equals = false;
            }

        // 파일이 달라진 경우
        if (!equals)
        {
            SaveVersionFile(versionFilePath, newFile);
            _version[filePath] = _currentVersion+1;
        }
    }

    private byte[] Hashing(string file)
    {
        var hash = new SHA1CryptoServiceProvider();
        var byteFile = Encoding.UTF8.GetBytes(file);
        return hash.ComputeHash(byteFile);
    }

    /// <summary>
    /// 해쉬 데이터 파일 생성
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="hash"></param>
    private void SaveVersionFile(string filePath, byte[] hash)
    {
        var directoryPath = filePath.Substring(0, filePath.LastIndexOf('/'));
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        using (var file = File.OpenWrite(filePath))
        {
            file.Write(hash, 0, hash.Length);
        }
    }
    
    /// <summary>
    /// 버젼 텍스트 파일 정보를 읽어온다.
    /// </summary>
    private void LoadVersionFile()
    {
        if (!File.Exists(AssetBundlePath.VersionTextPath))
        {
            _currentVersion = 0;
            return;
        }
        using (var file = File.OpenRead(AssetBundlePath.VersionTextPath))
        {
            var buffer = new byte[65535];
            var count = file.Read(buffer, 0, buffer.Length);
            var versionFile = Encoding.UTF8.GetString(buffer).Substring(0,count).Split('\n');
            _currentVersion = int.Parse(versionFile[0]);
            for (int i = 1; i < versionFile.Length; i++)
            {
                Debug.Log(versionFile[i].Length+"<<<");
                if (versionFile[i] == "" || versionFile[i] == null) continue;
                var versionInfo = versionFile[i].Split(':');
                _version.Add(versionInfo[0], int.Parse(versionInfo[1]));
            }
        }
    }

    /// <summary>
    /// 버전 텍스트 파일 정보를 업데이트
    /// </summary>
    public void UpdateVersionTextFile()
    {
        using (var versionFile = File.OpenWrite(AssetBundlePath.VersionTextPath))
        {
            _currentVersion++;
            var versionBytes = Encoding.UTF8.GetBytes(_currentVersion.ToString()+'\n');
            versionFile.Write(versionBytes,0,versionBytes.Length);
            foreach (var fileInfo in _version)
            {
                var str = fileInfo.Key + ':' + fileInfo.Value + '\n';
                var buffer = Encoding.UTF8.GetBytes(str);
                versionFile.Write(buffer, 0, buffer.Length);
            }
            versionFile.Flush();
        }
    }
}