using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;

public class MotionTrackerGUI : EditorWindow
{
    #region <GUI Field>

    private Vector2 _scrollPos;
    
    private float _scaleDelta;
    private float _showDistance;

    private Transform _actor;
    private Transform _tracker;

    private int _transformListSize;
    private List<TransformPair> _trackList;

    #endregion

    private void Awake()
    {
        Debug.Log("21212");
        _scaleDelta = 1f;
        _trackList = new List<TransformPair>();
    }

    [MenuItem("AnimationConverter/MotionTracker")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow(typeof(MotionTrackerGUI));
        var width = 800;
        var height = 800;
        window.position = new Rect((Screen.width-width)/2 , (Screen.height-height)/2, width, height);
        window.minSize = new Vector2(500,120);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("MotionTracker");
        _scaleDelta = EditorGUILayout.Slider("Scale Delta Value", _scaleDelta, 0f, 500f);
        _showDistance = EditorGUILayout.Slider("Model View Distance", _showDistance, -10f, 10f);

        _actor = EditorGUILayout.ObjectField("Actor", _actor, typeof(Transform), true) as Transform;
        _tracker = EditorGUILayout.ObjectField("Tracker", _tracker, typeof(Transform), true) as Transform;
        
        _transformListSize = EditorGUILayout.IntField("Track List Size", _transformListSize);
        
        ListSizeChange();
        
        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
        {
            _scrollPos = scrollView.scrollPosition;
            EditorGUI.indentLevel++;
            for (int i = 0; i < _transformListSize; i++)
            {
                var toggleText = _trackList[i].UnTrack ? "No Tracking" : "Element";
                _trackList[i].UnTrack = EditorGUILayout.ToggleLeft($" {toggleText} {i+1} -------------------------------------------------------------------------------------------------------------------", _trackList[i].UnTrack);
                EditorGUI.indentLevel++;
                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    _trackList[i].From = EditorGUILayout.ObjectField("Actor Part Transform", _trackList[i].From, typeof(Transform), true, GUILayout.Width(330)) as Transform;
                    EditorGUILayout.Space();
                    _trackList[i].AdjustmentRotation = EditorGUILayout.Vector3Field("Adjustment Rotation", _trackList[i].AdjustmentRotation, GUILayout.Width(330));
                    EditorGUILayout.Space();
                }
                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    _trackList[i].To = EditorGUILayout.ObjectField("Tracker Part Transform", _trackList[i].To, typeof(Transform), true, GUILayout.Width(330)) as Transform;
                    EditorGUILayout.Space();
                    
                    _trackList[i].AdjustmentPosition = EditorGUILayout.Vector3Field("Adjustment Position", _trackList[i].AdjustmentPosition,GUILayout.Width(330));
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
    }

    private void ListSizeChange()
    {
        if (_transformListSize == _trackList.Count) return;
        if (_transformListSize < _trackList.Count)
        {
            while (_transformListSize != _trackList.Count)
            {
                _trackList.RemoveAt(_trackList.Count - 1);
            }
        }
        else
        {
            while (_transformListSize != _trackList.Count)
            {
                _trackList.Add(new TransformPair());
            }
        }
    }
}