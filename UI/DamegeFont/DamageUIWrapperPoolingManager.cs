using UI2020;
using UnityEngine;

public class DamageUIWrapperPoolingManager : AbstractComponentPoolingManager<DamageUIWrapper>
{
    protected override void OnCreate(DamageUIWrapper obj)
    {
        obj.Init();
    }

    protected override void OnActive(DamageUIWrapper obj)
    {
        obj.Set();
        obj.gameObject.SetActive(true);
    }

    protected override void OnPooled(DamageUIWrapper obj)
    {
        obj.transform.localScale = Vector3.one;
        obj.gameObject.SetActive(false);
    }
}