
using System;
using System.Collections.Generic;
using UnityEngine;

public class MotionTracker : MonoBehaviour
{
    [SerializeField][Range(0.0f,100.0f)]
    private float ScaleFacter = 1.0f;
    [SerializeField][Range(-100.0f,100.0f)]
    private float ShowDistance = 1.0f;
    [SerializeField][Tooltip("복사하려는 오브젝트의 루트 트렌스폼")] private Transform Actor;
    [SerializeField][Tooltip("따라하는 오브젝트의 루트 트렌스폼")] private Transform Tracker;
    [SerializeField] private List<TransformPair> TrackList;

    private void Update()
    {
        if (Actor == null || Tracker == null) return;
        var distance = Vector3.left * ShowDistance;
        Tracker.position = Actor.position * ScaleFacter + distance;
        foreach (var list in TrackList)
        {
            if (list.From == null || list.To == null || list.UnTrack) continue; 
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