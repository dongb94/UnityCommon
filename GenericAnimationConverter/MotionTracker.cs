
using System;
using System.Collections.Generic;
using UnityEngine;

public class MotionTracker : MonoBehaviour
{
    [Range(0.0f,100.0f)]
    public float ScaleFacter = 1.0f;
    [Range(-100.0f,100.0f)]
    public float ShowDistance = 1.0f;
    public List<TransformPair> TrackList;

    private void Update()
    {
        var distance = Vector3.left * ShowDistance;
        foreach (var list in TrackList)
        {
            if (list.From == null || list.To == null) continue; 
            list.To.position = list.From.position * ScaleFacter + distance;
            list.To.rotation = list.From.rotation;
        }
    }
}

[Serializable]
public class TransformPair
{
    public bool UnTrack;
    public Transform From;
    public Transform To;
}