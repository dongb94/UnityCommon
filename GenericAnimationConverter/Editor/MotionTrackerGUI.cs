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

    private int _transformListSize;
    private List<Transform> _tl;

    #endregion

    private void Awake()
    {
        _scaleDelta = 1f;
        _tl = new List<Transform>();
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
        _showDistance = EditorGUILayout.Slider("Model View Distance", _showDistance, 0f, 10f);

        _transformListSize = EditorGUILayout.IntField("Track List Size", _transformListSize);
        
        ListSizeChange();
        
        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
        {
            _scrollPos = scrollView.scrollPosition;
            for (int i = 0; i < _transformListSize; i++)
            {
                _tl[i] = EditorGUILayout.ObjectField("Transform", _tl[i], typeof(Transform), true) as Transform;
            }
        }
    }

    private void ListSizeChange()
    {
        if (_transformListSize == _tl.Count) return;
        if (_transformListSize < _tl.Count)
        {
            while (_transformListSize != _tl.Count)
            {
                _tl.RemoveAt(_tl.Count - 1);
            }
        }
        else
        {
            while (_transformListSize != _tl.Count)
            {
                _tl.Add(null);
            }
        }
    }
}