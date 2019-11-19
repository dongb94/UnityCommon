/*

AnimationClipPath에 있는 모든 에니메이션들을 변환해 SavePath에 저장한다.
등록한 모델 파츠만 변환된다.

변환하려는 애니매이션 클립은 반드시 리소스 폴더 내부에 있어야 한다.

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using UnityEditor;
using UnityEngine;
public class GenericAnimationConverter : MonoBehaviour
{
    // start from /Assets
    public string AnimationClipPath;
    public string SaveFolderPath;

    public ConvertFactor ConvertFactor;
    public List<TransformPair> ConvertModelParts;

    public Animation sample;
    public AnimationClip clipsample;

    private Dictionary<string, string> ConvertPathPair;

    private void Awake()
    {
        string assetsPath = Application.dataPath+"/";

        if (AnimationClipPath[AnimationClipPath.Length - 1] != '/') AnimationClipPath += "/";
        if (SaveFolderPath[SaveFolderPath.Length - 1] != '/') SaveFolderPath += "/";
        if (!AnimationClipPath.Contains("Resources"))
        {
            Debug.LogError("변환하려는 애니매이션의 경로가 Resources폴더가 아닙니다.");
            return;
        }

        foreach (var transformPair in ConvertModelParts)
        {
            if(transformPair.New == null || transformPair.Old == null) continue;
            ConvertPathPair.Add(GetTransformPath(transformPair.Old), GetTransformPath(transformPair.New));
        }

        try
        {
            var oldClips = Directory.GetFiles(assetsPath + AnimationClipPath, "*.anim");

            foreach (var clip in oldClips)
            {
                var split = clip.Split(new []{'/','\\'});
                var fileName = split[split.Length-1];
                
                FileStream newAnimation;
                StreamReader oldAnimation = File.OpenText(clip);
                
                var clipFile = oldAnimation.ReadToEnd();

                foreach (AnimationState VARIABLE in sample)
                {
                    Debug.Log("name : "+VARIABLE.clip.name);
                }
                
                var resourcesPath = clip.Substring(clip.IndexOf("Resources") + 10);
                Debug.Log(resourcesPath);
                AnimationClip animationClip = Resources.Load<AnimationClip>(resourcesPath);
                sample.AddClip(clipsample, "sample");
                
                foreach (AnimationState VARIABLE in sample)
                {
                    Debug.Log("name : "+VARIABLE.clip.name);
                }

                
                // 편집된 파일을 저장
                byte[] buffer = Encoding.UTF8.GetBytes(ConvertAnimation(clipFile));

                if (!Directory.Exists(assetsPath + SaveFolderPath))
                {
                    Directory.CreateDirectory(assetsPath + SaveFolderPath);
                }
                
                if (!File.Exists(assetsPath + SaveFolderPath + fileName))
                {
                    newAnimation = File.Create(assetsPath + SaveFolderPath + fileName);
                    newAnimation.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    Debug.LogErrorFormat("{0} 경로에 파일이 이미 존재합니다.", clip);
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("해당 경로에 파일이 존재하지 않습니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (PathTooLongException e)
        {
            Debug.Log("경로명이 너무 깁니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (SecurityException e)
        {
            Debug.Log("권한이 없습니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.Log("권한이 없습니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("경로가 비었습니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log("경로가 잘못되었습니다.");
            Console.WriteLine(e);
            throw;
        }
        catch (IOException e)
        {
            Debug.Log("해당 경로는 파일입니다.");
            Console.WriteLine(e);
            throw;
        }
    }
    
    private string ConvertAnimation(string anim)
    {
        var RotationIndex = anim.IndexOf("m_RotationCurves:");
        var CompressedRotationIndex = anim.IndexOf("m_CompressedRotationCurves:");
        var EulerIndex = anim.IndexOf("m_EulerCurves:");
        var PositionIndex = anim.IndexOf("m_PositionCurves:");
        var ScaleIndex = anim.IndexOf("m_ScaleCurves:");
        var FloatIndex = anim.IndexOf("m_FloatCurves:");
        var PPtrIndex = anim.IndexOf("m_PPtrCurves:");
        var BoundsIndex = anim.IndexOf("m_Bounds:");
        var ClipBindingConstantIndex = anim.IndexOf("m_ClipBindingConstant:");
        var EditorIndex = anim.IndexOf("m_EditorCurves:");
        var EulerEditorIndex = anim.IndexOf("m_EulerEditorCurves:");
        
        var convertAnim = anim.Substring(0, RotationIndex);

        var rotationCurves = anim.Substring(RotationIndex, CompressedRotationIndex - RotationIndex);

        
        var state = sample["sample"];
        Debug.Log(state == null);
        //state.speed = 0;
        state.time = 0.5f;
        sample.Play();
        

        return convertAnim;
    }
    
    
    
    public string GetTransformPath(Transform obj)
    {
        string path = "";
        while(obj.parent != null)
        {
            path = obj.name + "/" + path;
            obj = obj.parent; //탐색 오브젝트를 부모로 갱신, 최상위 부모는 경로에 포함시키지 않음
        }

        if (path[path.Length - 1] == '/') path = path.Substring(0, path.Length - 1);
        return path;
    }
}

[Serializable]
public class TransformPair
{
    public Transform Old;
    public Transform New;
}

[Serializable]
public class ConvertFactor
{
    public bool RootMotion;
    public bool Rotation;
    public bool Scale;
}

