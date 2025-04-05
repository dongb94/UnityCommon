# Unity Cursor
## 사용법
1. 에셋에서 CursorTextureSO Scriptable Object추가
2. Scriptable Object에 각 상태에 맞는 텍스쳐 추가
3. 빈 오브젝트 생성 후 CursorManager 추가 및 생성한 Scriptable Object 등록

## 사용 예시

    public class CursorControl : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CursorManager.Instance.SetCursor(CursorType.Click);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                CursorManager.Instance.SetCursor(CursorType.Default);
            }
        }
    }