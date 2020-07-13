using System.Collections.Generic;
using k514;
using UnityEngine;
using UnityEngine.UI;

///
/// print damage UI (UGUI)
///
/// material :
/// 1. DamageUI MultiSprite
/// 2. UI GameObject
///
///
public class DamageUI : UIManagerBase
{
    public static DamageUI Instance;

    public Sprite[] damageFont; // base multi sprite

    public enum DamageType
    {
        Crit,
        Hurt,
        Normal
    }

    private const int FloatTime = 2;
    private const int FloatingHeight = 120;
    private const int Interval = 30;
    private const float CritEffectSize = 1.5f;

    private Queue<GameObject> FontWrapper;

    private void Awake()
    {
        Instance = this;
        damageFont = Resources.LoadAll<Sprite>("UI2020/Sprites/FontImage/DamageFontSprite2"); // TODO 개선 해야함
        gameObject.AddComponent<DamageUIWrapperPoolingManager>().Initialize();
        gameObject.AddComponent<DamageUIImagePoolingManager>().Initialize();
    }

    public void printDamage(int damage, Vector3 position, DamageType damageType = DamageType.Normal)
    {
        if (damage < 0)
        {
            Debug.LogError($"Input minus damage {damage}");
        }

        var mainCamera = CameraManager.GetInstance.MainCamera;
        
        // 카메라 기준 데미지를 표기할 월드좌표 벡터
        var printVector = (position - mainCamera.transform.position);
        // 카메라가 보고있는 방향 벡터
        var cameraVector = mainCamera.transform.forward;
        // 내적을 이용해서 카메라 앞인지 뒤인지를 판별한다. 뒤쪽이면 리턴.
        if (printVector.x * cameraVector.x 
            + printVector.y * cameraVector.y 
            + printVector.z * cameraVector.z < 0) return;

        // var screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
        var screenPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, position);
        screenPosition += new Vector2(0, FloatingHeight);
        // Debug.Log("Screen H W : "+ Screen.width+" x "+Screen.height);
        // Debug.Log(position +" ====> " + screenPosition);

        var wrapper = DamageUIWrapperPoolingManager.Instance.GetObject();
        wrapper.transform.position = screenPosition;
        wrapper.SetDamage(damage, damageType, FloatTime);
        wrapper.AddAnimation(DamageUIAnimations.Floating(wrapper.transform, FloatingHeight, FloatTime))
            .AddAnimation(DamageUIAnimations.SizeHighlighting(wrapper.transform, Vector3.one * 0.3f, Vector3.one * 0.5f, 0.05f, 1.2f))
            //.AddAnimation(DamageUIAnimations.SizeHighlighting(wrapper.transform, Vector3.one, Vector3.one * 2f, 0.2f, 0.8f))
            .AddAnimation(DamageUIAnimations.Shaking(wrapper.transform, 15f, 4, 0.7f))
            .AddAnimation(DamageUIAnimations.Dissolve(wrapper.activeGroup, 2f, 1.0f));
        wrapper.StartAnimation();
    }
    public override void OnUpdateUI(float p_DeltaTime)
    {
    }
    public override void DisposeUnManaged()
    {
    }
}
