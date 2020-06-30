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
        
        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos))
        {
            _scrollPos = scrollView.scrollPosition;
            EditorGUILayout.ObjectField("Transform", null, typeof(List<Transform>), true);
        }
    }
    
}


public class MyWindow : EditorWindow
{
    AnimBool m_ShowExtraFields;
    string m_String;
    Color m_Color = Color.white;
    int m_Number = 0;

    [MenuItem("Window/My Window")]
    static void Init()
    {
        MyWindow window = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
        window.Show();
    }

    void OnEnable()
    {
        m_ShowExtraFields = new AnimBool(true);
        m_ShowExtraFields.valueChanged.AddListener(new UnityAction(base.Repaint));
    }

    void OnGUI()
    {
        m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("Show extra fields", m_ShowExtraFields.target);

        //Extra block that can be toggled on and off.
        using (var group = new EditorGUILayout.FadeGroupScope(m_ShowExtraFields.faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel("Color");
                m_Color = EditorGUILayout.ColorField(m_Color);
                EditorGUILayout.PrefixLabel("Text");
                m_String = EditorGUILayout.TextField(m_String);
                EditorGUILayout.PrefixLabel("Number");
                m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10);
                EditorGUI.indentLevel--;
            }
        }
    }
}