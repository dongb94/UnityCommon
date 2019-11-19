
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AnimationWriter
{
    public static int SampleRate = 30;
    private static MotionData Data;

    enum TransformPart
    {
        Position,
        Rotation,
        Euler,
        Scale
    }
    
    #region Make Animation File
    public static void MakeAnimation(string AnimationSavePath, string AnimationName, ref MotionData data)
    {
        Data = data;
        
        var clipFile = "%YAML 1.1\n" +
                       "%TAG !u! tag:unity3d.com,2011:\n" +
                       "--- !u!74 &7400000\n" +
                       "AnimationClip:\n" +
                       "  m_ObjectHideFlags: 0\n" +
                       "  m_CorrespondingSourceObject: {fileID: 0}\n" +
                       "  m_PrefabInstance: {fileID: 0}\n" +
                       "  m_PrefabAsset: {fileID: 0}\n" +
                       "  m_Name: " + AnimationName + "\n" +
                       "  serializedVersion: 6\n" +
                       "  m_Legacy: 0\n" +
                       "  m_Compressed: 0\n" +
                       "  m_UseHighQualityCurve: 1\n" +
                       "  m_RotationCurves: []\n" +
                       "  m_CompressedRotationCurves: []\n" +
                       "  m_EulerCurves:" +
                       GetCurves(TransformPart.Euler) + 
                       "  m_PositionCurves:" + 
                       GetCurves(TransformPart.Position) + 
                       "  m_ScaleCurves:" +
                       GetCurves(TransformPart.Scale) +
                        "  m_FloatCurves: []\n" +
                        "  m_PPtrCurves: []\n" +
                        "  m_SampleRate: " + SampleRate + "\n" +
                        "  m_WrapMode: 0\n" +
                        "  m_Bounds:\n" +
                        "    m_Center: {x: 0, y: 0, z: 0}\n" +
                        "    m_Extent: {x: 0, y: 0, z: 0}\n" +
                        "  m_ClipBindingConstant: []\n" +
                        "  m_AnimationClipSettings:\n" +
                        "    serializedVersion: 2\n" +
                        "    m_AdditiveReferencePoseClip: {fileID: 0}\n" +
                        "    m_AdditiveReferencePoseTime: 0\n" +
                        "    m_StartTime: 0\n" +
                        "    m_StopTime: 1\n" +
                        "    m_OrientationOffsetY: 0\n" +
                        "    m_Level: 0\n" +
                        "    m_CycleOffset: 0\n" +
                        "    m_HasAdditiveReferencePose: 0\n" +
                        "    m_LoopTime: 1\n" +
                        "    m_LoopBlend: 0\n" +
                        "    m_LoopBlendOrientation: 0\n" +
                        "    m_LoopBlendPositionY: 0\n" +
                        "    m_LoopBlendPositionXZ: 0\n" +
                        "    m_KeepOriginalOrientation: 0\n" +
                        "    m_KeepOriginalPositionY: 1\n" +
                        "    m_KeepOriginalPositionXZ: 0\n" +
                        "    m_HeightFromFeet: 0\n" +
                        "    m_Mirror: 0\n" +
                        "  m_EditorCurves: []\n" +
                        "  m_EulerEditorCurves: []\n" +
                        "  m_HasGenericRootTransform: 1\n" +
                        "  m_HasMotionFloatCurves: 0\n" +
                        "  m_Events: []\n";
        
        
        var assetsPath = Application.dataPath+"/";
        byte[] buffer = Encoding.UTF8.GetBytes(clipFile);

        if (!Directory.Exists(assetsPath + AnimationSavePath))
        {
            Directory.CreateDirectory(assetsPath + AnimationSavePath);
        }
                
        if (!File.Exists(assetsPath + AnimationSavePath + AnimationName))
        {
            var newAnimation = File.Create(assetsPath + AnimationSavePath + "/" + AnimationName + ".anim");
            newAnimation.Write(buffer, 0, buffer.Length);
        }
        else
        {
            Debug.LogErrorFormat("{0} 경로에 파일이 이미 존재합니다.", AnimationName);
        }
    }
    
    private static string GetCurves(TransformPart part)
    {
        var useQuaternion = false;
        var curve = "\n";
        foreach (var motionData in Data.TimeMotionDatas)
        {
            List<VectorTimeSet> vectors;
            switch (part)
            {
                case TransformPart.Position :
                    vectors = motionData.Value.Position;
                    break;
                case TransformPart.Rotation :
                    vectors = motionData.Value.Rotation;
                    useQuaternion = true;
                    break;
                case TransformPart.Euler :
                    vectors = motionData.Value.Rotation;
                    break;
                case  TransformPart.Scale :
                    vectors = motionData.Value.Scale;
                    break;
                default :
                    throw new IndexOutOfRangeException();
            }
            if(vectors.Count == 0) return " []\n"; 
                
            curve += "  - curve:\n" +
                     "      serializedVersion: 2\n" +
                     "      m_Curve:\n";

            foreach (var vector in vectors)
            {
                float x, y, z, w;
                if (useQuaternion)
                {
                    var quaternion = Quaternion.Euler(vector.Vector3);
                    x = quaternion.x;
                    y = quaternion.y;
                    z = quaternion.z;
                    w = quaternion.w;
                }
                else
                {
                    x = vector.Vector3.x;
                    y = vector.Vector3.y;
                    z = vector.Vector3.z;
                    w = 0;
                }
                
                curve += "      - serializedVersion: 3\n" +
                         "        time: " + vector.Time + "\n" +
                         "        value: {" + String.Format("x: {0}, y: {1}, z: {2}", x, y, z) + (useQuaternion?String.Format(", w: {0}",w):"") + "}\n" +
                         "        inSlope: {x: 0, y: 0, z: 0}\n" +
                         "        outSlope: {x: 0, y: 0, z: 0}\n" +
                         "        tangentMode: 0\n" +
                         "        weightedMode: 0\n" +
                         "        inWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}\n" +
                         "        outWeight: {x: 0.33333334, y: 0.33333334, z: 0.33333334}\n";
            }

            curve += "      m_PreInfinity: 2\n" +
                     "      m_PostInfinity: 2\n" +
                     "      m_RotationOrder: 4\n" +
                     "    path: " + motionData.Key + "\n";
            
        }
        return curve;
    }
    
    #endregion
}