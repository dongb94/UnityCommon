using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 유니티 상의 동작을 캡쳐해 에니메이션 파일로 만들어 줍니다.
/// 선택된 게임 오브젝트 기준으로 모든 자식의 Transform을 캡쳐합니다.
/// 기본 저장 경로는 프로젝트의 Assets폴더입니다.
/// 유니티 상에서 애니메이션 파일을 확인하려면 약간의 시간이 소요됩니다.
/// 
/// 캡쳐하려는 오브젝트에 부모가 있으면 오작동 할 수 있습니다.
/// 캡쳐하려는 오브젝트에 경로가 같은 자식 오브젝트가 존재하면 작동하지 않습니다.
/// </summary>
public class UnityMotionCapture : MonoBehaviour
{
    // properties
    public GameObject CaptureGameObject;
    public String AnimationSavePath;
    public String AnimationName;
    public int SampleRate = 30;
    public bool UseQuerternion;
    public bool IgnoreScale;

    // ui
    private Button _startButton;
    private Text _text;

    // factor
    private bool _isRecording = false;
    
    private MotionData _capturedMotion;
    
    //timer
    private float _startTime;
    private float _lastCaptureTime;
    private float _delay;

    private void Start()
    {
        _capturedMotion = new MotionData();
        
        _startButton = FindObjectOfType<Button>();
        if (_startButton == null) // UI세팅이 안되있는 경우
        {
            _startButton = new GameObject("Button").AddComponent<Button>();
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                canvas = new GameObject("Canvas").AddComponent<Canvas>();
                canvas.gameObject.AddComponent<CanvasScaler>();
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _startButton.transform.parent = canvas.transform;
            var image = _startButton.gameObject.AddComponent<Image>();
            _startButton.targetGraphic = image;
            var rectTransform = image.rectTransform;
            rectTransform.anchorMin = rectTransform.anchorMax = Vector2.right;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);
            rectTransform.anchoredPosition = new Vector2(-150, 50);
        }
        _text = _startButton.gameObject.GetComponentInChildren<Text>();
        if (_text == null)
        {
            _text = new GameObject("Text").AddComponent<Text>();
            _text.transform.parent = _startButton.transform;
            _text.transform.localPosition = Vector3.zero;
            _text.alignment = TextAnchor.MiddleCenter;
            _text.color = Color.black;
            _text.fontSize = 20;
            _text.font =Font.CreateDynamicFontFromOSFont("Arial", 20);
        }
        _text.text = "Start";
        if (!FindObjectOfType<EventSystem>())
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        
        _startButton.onClick.AddListener(OnClick);

        if (AnimationName == "") AnimationName = CaptureGameObject.name;
    }

    private void Update()
    {
        if (!_isRecording) return;

        var currTime = Time.time;
        if (currTime - _lastCaptureTime > _delay)
        {
            _lastCaptureTime = currTime;
            Recording();
        }
    }

    private void OnClick()
    {
        if (CaptureGameObject == null)
        {
            Debug.LogError("CaptureGameObject is " + CaptureGameObject);
            return;
        }

        if (_isRecording)
        {
            _isRecording = false;
            _text.text = "Start";
            SaveAnimation();
        }
        else
        {
            ReadTransform();
            AnimationWriter.SampleRate = SampleRate;
            _delay = 1f / SampleRate;
            _startTime = Time.time;
            _lastCaptureTime = _startTime;
            _isRecording = true;
            _text.text = "Recoding";
            Recording();
        }
    }

    private void ReadTransform()
    {
        var captureParts = CaptureGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (var trs in captureParts)
        {
            _capturedMotion.AddTransform(trs);
        }
    }
    
    private void Recording()
    {
        _capturedMotion.Capture(_lastCaptureTime-_startTime, IgnoreScale);
    }
    
    private void SaveAnimation()
    {
        AnimationWriter.MakeAnimation(AnimationSavePath,AnimationName,UseQuerternion, ref _capturedMotion);
        _capturedMotion.Clear();
    }
}