// UGUI Canvas의 Render Mode가 'Screen Space - Overlay' 일 경우
// GraphicRaycaster 컴포넌트를 통해 마우스 이벤트를 받는 예시
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicRaycasterEx : MonoBehaviour
{
    // 예시에서 사용할 GraphicRaycaster객체
    private GraphicRaycaster gr;

    private void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();
    }

    private void Update()
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        if (results.Count <= 0) return;
        // 이벤트 처리부분
        results[0].gameObject.transform.position = ped.position;
    }
}
