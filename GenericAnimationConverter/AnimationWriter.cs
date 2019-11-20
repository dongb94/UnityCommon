
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
    public static void MakeAnimation(string AnimationSavePath, string AnimationName, bool UseQuerternion, ref MotionData data)
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
                       "  m_RotationCurves: " +
                       (!UseQuerternion?" []\n":GetCurves(TransformPart.Rotation)) +
                       "  m_CompressedRotationCurves: []\n" +
                       "  m_EulerCurves:" +
                       (UseQuerternion?" []\n":GetCurves(TransformPart.Euler)) + 
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
                        "    m_StopTime: " + Data.Time + "\n" +
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
            newAnimation.Close();
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

            var checkDirection = Vector3.zero;
            var checkVector = Vector3.zero;
            for (int i = 0; i<vectors.Count; i++)
            {
                var vector = vectors[i];
                float x, y, z, w;

                if (part == TransformPart.Euler || part == TransformPart.Rotation)
                {
                    if (checkVector != Vector3.zero)
                    {
                        vector.Vector3 = CheckSmoothRotation(checkVector, vector.Vector3, checkDirection);
                        checkDirection.x = vector.Vector3.x > checkVector.x ? 1 : -1;
                        checkDirection.y = vector.Vector3.y > checkVector.y ? 1 : -1;
                        checkDirection.z = vector.Vector3.z > checkVector.z ? 1 : -1;
                    }
                    checkVector = vector.Vector3;
                }
                
                if (useQuaternion)
                {
                    var vector3 = vector.Vector3;
                    var quaternion = Quaternion.Euler(vector3);
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

    private static Vector3 CheckSmoothRotation(Vector3 from, Vector3 to, Vector3 direction)
    {
        var f = to;
        if ((from.x - to.x > 180 && direction.x > 0) || from.x - to.x > 350) to.x += 360;
        else if ((from.x - to.x < -180 && direction.x < 0) || from.x - to.x < -350) to.x -= 360;
        if ((from.y - to.y > 180 && direction.y > 0)|| from.y - to.y > 350) to.y += 360;
        else if ((from.y - to.y < -180 && direction.y < 0) || from.y - to.y < -350) to.y -= 360;
        if ((from.z - to.z > 180 && direction.z > 0)  || from.z - to.z > 350) to.z += 360;
        else if ((from.z - to.z < -180 && direction.z < 0)  || from.z - to.z < -350) to.z -= 360;
        
        //debug;
        if(Math.Abs(from.x - to.x) > 100 || Math.Abs(from.y - to.y) > 100 || Math.Abs(from.z - to.z) > 100)
            Debug.Log("x : "+ from.x +" > "+f.x+" > "+to.x + "\n" + "y : "+ from.y +" > "+f.y+" > "+to.y + "\n" + "z : "+ from.z +" > "+f.z+" > "+to.z + "\n");

        return to;
    }
}