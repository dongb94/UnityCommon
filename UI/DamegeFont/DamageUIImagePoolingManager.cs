using UI2020;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIImagePoolingManager : AbstractComponentPoolingManager<Image>
{
    protected override void OnCreate(Image obj)
    {
        obj.gameObject.name = "damageFont";
    }

    protected override void OnActive(Image obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected override void OnPooled(Image obj)
    {
        obj.color = Color.white;
        obj.gameObject.SetActive(false);
    }
}