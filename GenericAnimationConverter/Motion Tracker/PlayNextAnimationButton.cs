
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayNextAnimationButton : MonoBehaviour
{
    private Button _button;
    private Animator _animator;

    private static int State = 0;
    private AnimatorClipInfo lastState;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _animator = GameObject.Find("Actor")?.GetComponent<Animator>();
        _button.onClick.AddListener(OnClick);
        _animator.SetInteger("State",-1);
    }

    public void OnClick()
    {
        State = (State + 1) % 28;
        _animator.SetInteger("State",State);
    }

    private void LateUpdate()
    {
        var info = _animator.GetCurrentAnimatorClipInfo(0)[0];
        if (info.Equals(lastState)) return;
        _animator.SetInteger("State",-1);
        lastState = info;
        var capture = FindObjectOfType<UnityMotionCapture>();
        if (capture != null) capture.AnimationName = info.clip.name;
    }
}