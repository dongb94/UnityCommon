
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 에셋번들을 분할해 압축하는 클레스
/// </summary>
public class AssetBundleCompressor
{
    public int nOfFile;
    public int progress;
    
    private static AssetBundleCompressor _instance;
    
    private readonly string BundlePath;
    private readonly string ZipPath;
    
    public static AssetBundleCompressor Instance => _instance??new AssetBundleCompressor();

    public async Task<string> MakeAssetBundleZipFile()
    {
        var manifestList = SearchManifest();
        if (manifestList == null) return "지정된 경로에서 에셋번들을 찾을 수 없습니다.";
        return BundleCompression(manifestList).Result;
    }

    public void Delete()
    {
        _instance = null;
    }
    
    private AssetBundleCompressor()
    {
        _instance = this;
        nOfFile = 0;
        progress = 0;
        BundlePath = AssetBundlePath.BundlePath;
        ZipPath = AssetBundlePath.SVNPath;
    }

    /// <summary>
    /// 에셋번들 폴더 내부에 있는 모든 메니페스트 파일 경로를 반환
    /// </summary>
    /// <returns></returns>
    private List<string> SearchManifest()
    {
        // 에셋 번들이 없거나 경로가 잘못되었음
        if (!Directory.Exists(BundlePath)) return null;

        var pathList = new List<string>(); // 반환할 Manifest file list
        var searchPathQueue = new Queue<string>(); // 탐색 큐
        searchPathQueue.Enqueue(BundlePath);

        while (searchPathQueue.Count > 0)
        {
            var curPath = searchPathQueue.Dequeue();
            var directories = Directory.GetDirectories(curPath);
            var files = Directory.GetFiles(curPath);
            
            foreach (var directory in directories) searchPathQueue.Enqueue(directory);

            foreach (var file in files) if(file.EndsWith(AssetBundlePath.MANIFEST_FILE_EXTENSION)) pathList.Add(file);
        }

        return pathList;
    }

    private async Task<string> BundleCompression(List<string> manifestPath)
    {
        nOfFile = manifestPath.Count;
        progress = 0;
        for (var i = 0; i < manifestPath.Count; i++)
        {
            progress = i;
            var fileName = manifestPath[i].Substring(BundlePath.Length);
            fileName = fileName.Replace('\\', '/');
            fileName = fileName.Remove(fileName.IndexOf(AssetBundlePath.MANIFEST_FILE_EXTENSION, StringComparison.Ordinal));

            var fileLocalPath = fileName;
            var fullPath = $"{ZipPath}{fileName}{AssetBundlePath.ZIP_FILE_EXTENSION}";
            var directoryPath = fullPath.Substring(0, fullPath.LastIndexOf('/'));

            fileName = fileName.Substring(fileName.LastIndexOf('/')+1);

            string file = "";
            
            Debug.Log(fileName);
            Debug.Log(fullPath);

            try
            {
                CheckDirectory(directoryPath);
                using (var zipFile = ZipFile.Open(fullPath, ZipArchiveMode.Update))
                {
                    var buffer = new byte[65536];
                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 1) fileName += AssetBundlePath.MANIFEST_FILE_EXTENSION;
                        var bundleEntry = zipFile.GetEntry(fileName);
                        if (bundleEntry == null)
                            bundleEntry = zipFile.CreateEntry(fileName);
                        var zipStream = bundleEntry.Open();
                        FileStream fileStream;
                        if (j == 0)
                            fileStream =
                                File.OpenRead(manifestPath[i].Remove(manifestPath[i].IndexOf(AssetBundlePath.MANIFEST_FILE_EXTENSION)));
                        else
                            fileStream = File.OpenRead(manifestPath[i]);

                        int count;
                        while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            zipStream.Write(buffer, 0, count);
                            file += Encoding.UTF8.GetString(buffer);
                        }

                        zipStream.Flush();

                        zipStream.Close();
                        fileStream.Close();
                    }
                }
            }
            catch (PathTooLongException e)
            {
                Console.WriteLine(e);
                return "경로가 너무 깁니다.";
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return "파일이 열려있거나 권한이 없습니다.";
            }
            
            AssetBundleVersionManager.instance.CheckVersionUp(fileLocalPath, file);
        }
        
        AssetBundleVersionManager.instance.UpdateVersionTextFile();
        AssetBundleVersionManager.instance.Delete();
        return "에셋번들 압축 완료";
    }

    private void CheckDirectory(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}