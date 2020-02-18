using UnityEngine;

namespace Almond.Util
{
    public class ScreenSizeFreeObjectHandler : MonoBehaviour
    {
        [SerializeField]
        private int BaseScreenHeight = Screen.height;

        private void Awake()
        {
            transform.localScale *= (float)Screen.height / BaseScreenHeight;
        }
    }
}