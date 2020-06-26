using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIWrapper : MonoBehaviour
{
    public List<Image> activeGroup;

    private Action<float, float> damageUIAnimation;

    private bool _isActive;
    private float _endTime;
    private float _time;
    
    private void LateUpdate()
    {
        if (!_isActive) return;
        var deltaTime = Time.deltaTime;
        damageUIAnimation(_time, deltaTime);
        _time += Time.deltaTime;
        if(_time > _endTime) EndAnimation();
    }

    /// <summary>
    /// 최초 초기화
    /// </summary>
    public void Init()
    {
        activeGroup = new List<Image>();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public void Set()
    {
        _isActive = false;
        _time = 0f;
        activeGroup.Clear();
        ResetAnimation();
    }

    public void SetDamage(int damage, DamageUI.DamageType damageType, float floatingTime)
    {
        _endTime = floatingTime;
        
        var damageStack = new Stack<int>(); // can swap byte
        if(damage == 0) damageStack.Push(0);
        while (damage > 0)
        {
            damageStack.Push(damage%10);
            damage /= 10;
        }
        
        var distance = 0f;
        while (damageStack.Count > 0)
        {
            var nextNum = damageStack.Pop();
            var image = DamageUIImagePoolingManager.Instance.GetObject();

            image.sprite = DamageUI.Instance.damageFont[nextNum+10*(int)damageType];
            image.transform.parent = this.transform;
            
            // 대충 크기 조정 하는 부분이라는 뜻
            image.rectTransform.anchoredPosition = new Vector2(distance, 0);
            image.rectTransform.localScale = new Vector2(image.sprite.rect.width / 100, image.sprite.rect.height / 100); // 이미지의 rectTransform이 기본적으로 100 100의 크기를 가지고 있다. localScale과 rectTransform의 width, height가 중복되어서 계산되기 때문에 100을 나눠준다.
            if (damageType == DamageUI.DamageType.Hurt)
            {
                image.rectTransform.localScale *= 2;
                distance += image.sprite.rect.width;
            }
            distance += image.sprite.rect.width;

            activeGroup.Add(image);
        }
    }

    /// <summary>
    /// 데미지 표시를 시작한다. 반드시 호출해야함.
    /// </summary>
    public void StartAnimation()
    {
        _isActive = true;
    }

    /// <summary>
    /// 데미지 애니메이션을 일시 정지한다.
    /// </summary>
    /// <remark>
    /// <see cref="StartAnimation()"/> 을 통해 중지된 부분부터 다시 실행시킬 수 있다.
    /// </remark>
    public void StopAnimation()
    {
        _isActive = false;
    }

    /// <summary>
    /// 데미지 애니메이션을 종료하고 이미지와 레퍼 클레스를 풀링 시킨다.
    /// </summary>
    public void EndAnimation()
    {
        StopAnimation();
        foreach (var image in activeGroup)
        {
            DamageUIImagePoolingManager.Instance.PoolObject(image);
        }
        DamageUIWrapperPoolingManager.Instance.PoolObject(this);
    }

    /// <summary>
    /// 에니메이션 설정을 초기화 한다.
    /// </summary>
    public void ResetAnimation()
    {
        damageUIAnimation = null;
    }

    /// <summary>
    /// 에니메이션을 설정한다.
    /// </summary>
    /// <remarks>
    /// <para><see cref = "DamageUIAnimations"/>에서 원하는 액션을 가져올 수 있다.</para>
    /// </remarks>
    /// <param name="action">추가 애니메이션</param>
    /// <example>
    /// <code>
    /// AddAnimation(DamageUIAnimations.SampleAnimation());
    /// </code>
    /// </example>
    public DamageUIWrapper AddAnimation(Action<float,float> action)
    {
        damageUIAnimation += action;
        return this;
    }
}