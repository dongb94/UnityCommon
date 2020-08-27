using System;
using UI2020.UIComponent;
using UnityEngine;

public class DialogEventTrigger : UIButton
{
    #region <Callbacks>

    public void Initialize()
    {
        AddEvent(OnClick);
    }
    
    #endregion

    #region <Callback/Button>
    
    public void OnClick()
    {
        DialogManager.GetInstance.NextEvent();
        // SoundManager.GetInstance.Play_UI_Sfx(K514SfxStorage.BeepType.Touch);
    }

    #endregion
}