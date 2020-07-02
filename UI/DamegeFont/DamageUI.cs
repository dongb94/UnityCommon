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

    private Queue<Image> objectPool;
    private Queue<Image> activeGroup;

    private Queue<GameObject> FontWrapper;

    private void Awake()
    {
        Instance = this;
        damageFont = Resources.LoadAll<Sprite>("UI2020/Sprites/FontImage/DamageFontSprite"); // TODO 개선 해야함
        objectPool = new Queue<Image>();
        activeGroup = new Queue<Image>();
        gameObject.AddComponent<DamageUIWrapperPoolingManager>().Initialize();
        gameObject.AddComponent<DamageUIImagePoolingManager>().Initialize();
    }

    private void LateUpdate()
    {
        foreach (var image in activeGroup)
        {
            image.transform.position += Vector3.up * 2;
        }
    }

    public void printDamage(int damage, Vector3 position, DamageType damageType = DamageType.Normal)
    {
        if (damage < 0)
        {
            Debug.LogError($"Input minus damage {damage}");
        }
        
        // var screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
        var screenPosition = RectTransformUtility.WorldToScreenPoint(CameraManager.GetInstance.MainCamera, position);
        screenPosition += new Vector2(0, FloatingHeight);
        // Debug.Log("Screen H W : "+ Screen.width+" x "+Screen.height);
        // Debug.Log(position +" ====> " + screenPosition);

        var wrapper = DamageUIWrapperPoolingManager.Instance.GetObject();
        wrapper.transform.position = screenPosition;
        wrapper.SetDamage(damage, damageType, FloatTime);
        wrapper.AddAnimation(DamageUIAnimations.Floating(wrapper.transform, FloatingHeight, FloatTime))
            .AddAnimation(DamageUIAnimations.SizeHighlighting(wrapper.transform, Vector3.one, Vector3.one * 2f, 0.05f, 1.2f))
            .AddAnimation(DamageUIAnimations.SizeHighlighting(wrapper.transform, Vector3.one, Vector3.one * 2f, 0.2f, 0.8f))
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
