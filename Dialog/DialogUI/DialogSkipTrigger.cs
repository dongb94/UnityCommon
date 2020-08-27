using UI2020.UIComponent;
using UnityEngine;

public class DialogSkipTrigger : UIButton
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
        DialogManager.GetInstance.ExitDialog();
        // SoundManager.GetInstance.Play_UI_Sfx(K514SfxStorage.BeepType.Skip);
    }
    
    #endregion
}