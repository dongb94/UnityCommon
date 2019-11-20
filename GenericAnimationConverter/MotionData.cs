
using System;
using System.Collections.Generic;
using UnityEngine;

public class MotionData
{
    public float Time;
    public Dictionary<string, Transform> CaptureTransforms;
    public Dictionary<string, TimeMotionData> TimeMotionDatas;

    public MotionData()
    {
        CaptureTransforms = new Dictionary<string, Transform>();
        TimeMotionDatas = new Dictionary<string, TimeMotionData>();
    }
    public void AddTransform(Transform trs)
    {
        var path = GetTransformPath(trs);
        if (CaptureTransforms.ContainsKey(path))
        {
            Debug.LogErrorFormat("경로가 같은 오브젝트가 존재합니다. : {0}", path);
            throw new ArgumentException();
        }
        CaptureTransforms.Add(path,trs);
        TimeMotionDatas.Add(path, new TimeMotionData()
        {
            Position = new List<VectorTimeSet>(),
            Rotation = new List<VectorTimeSet>(),
            Scale = new List<VectorTimeSet>(),
        });
    }

    public void Capture(float time, bool ignoreScale = false)
    {
        if (time > Time) Time = time;
        foreach (var pair in CaptureTransforms)
        {
            var motionData = TimeMotionDatas[pair.Key];
            motionData.Position.Add(new VectorTimeSet()
            {
                Time = time,
                Vector3 = pair.Value.localPosition
            });
            motionData.Rotation.Add(new VectorTimeSet()
            {
                Time = time,
                Vector3 = pair.Value.localEulerAngles
            });
            if (ignoreScale) continue;
            motionData.Scale.Add(new VectorTimeSet()
            {
                Time = time,
                Vector3 = pair.Value.localScale
            });
        }
    }

    public void Clear()
    {
        CaptureTransforms.Clear();
        TimeMotionDatas.Clear();
    }

    private string GetTransformPath(Transform obj)
    {
        string path = "";
        while(obj.parent != null)
        {
            path = obj.name + "/" + path;
            obj = obj.parent; //탐색 오브젝트를 부모로 갱신, 최상위 부모는 경로에 포함시키지 않음
        }

        if (path.Length == 0) return "";
        if (path[path.Length - 1] == '/') path = path.Substring(0, path.Length - 1);
        return path;
    }
}

public struct TimeMotionData
{
    public List<VectorTimeSet> Position;
    public List<VectorTimeSet> Rotation;
    public List<VectorTimeSet> Scale;
}

public struct VectorTimeSet
{
    public float Time;
    public Vector3 Vector3;
}