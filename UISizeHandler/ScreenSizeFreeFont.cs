using UnityEngine;
using UnityEngine.UI;

namespace Almond.Util
{
    /// <summary>
    /// BaseScreenHeight 기준으로 작업하면 화면 해상도에 상관없이 폰트 크기가 맞춰진다.
    /// </summary>
    public class ScreenSizeFreeFont : Text
    {
        [SerializeField]
        private int BaseScreenHeight = Screen.height;

        protected override void Awake()
        {
            base.Awake();
            fontSize = (int) (fontSize * ((float)Screen.height / BaseScreenHeight));
        }
    }
}