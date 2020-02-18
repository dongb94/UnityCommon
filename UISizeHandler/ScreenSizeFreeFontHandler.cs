using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Almond.Util
{
    /// <summary>
    /// 텍스트 컴포넌트가 있는 오브젝트에 붙이고
    /// BaseScreenHeight 기준으로 작업하면 화면 해상도에 상관없이 폰트 크기가 맞춰진다.
    /// </summary>
    public class ScreenSizeFreeFontHandler : MonoBehaviour
    {
        [SerializeField]
        private int BaseScreenHeight = Screen.height; // 315

        protected void Awake()
        {
            var textComponent = GetComponent<Text>();
            if(textComponent!=null)
                textComponent.fontSize = (int) (textComponent.fontSize * ((float)Screen.height / BaseScreenHeight));
            var textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            if(textMeshProUGUI!=null)
                textMeshProUGUI.fontSize *= ((float)Screen.height / BaseScreenHeight);
            var textMeshPro = GetComponent<TextMeshPro>();
            if(textMeshPro!=null)
                textMeshPro.fontSize *= ((float)Screen.height / BaseScreenHeight);
        }
    }
}