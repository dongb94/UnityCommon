using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI2020.UIComponent
{
    public class UIButton : AbstractUI
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void AddEvent(UnityAction action)
        {
            _button.onClick.AddListener(action);
        }

        public virtual void Selected() {}
        public virtual void Unselected() {}
    }
}