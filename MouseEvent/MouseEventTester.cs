
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class MouseEventTester : MonoBehaviour
{
    private Camera _uiCamera;
    private Collider _collider;

    private bool _isMouseOver;
    private bool _isMousePushed;
    
    private void Awake()
    {
        _uiCamera = FindObjectOfType<Camera>(); // 해당 오브젝트를 비추고 있는 카메라
        _collider = GetComponent<Collider>(); // 오브젝트의 Collider
    }

    private void Update()
    {
        var ray = _uiCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        
        Debug.Log(hit.collider);

        var leftMouse = (int)MouseButton.LeftMouse; // Test를 위해 왼쪽 버튼을 입력으로 받는다.

        if (hit.collider != null && hit.collider == _collider && !_isMouseOver)
        {
            _isMouseOver = true;
            OnMouseEnter();
        }else if (hit.collider != _collider && _isMouseOver)
        {
            _isMouseOver = false;
            OnMouseExit();
        }

        if (_isMouseOver) OnMouseOver();
        if (_isMouseOver && Input.GetMouseButtonDown(leftMouse))
        {
            _isMousePushed = true;
            OnMouseDown();
        }
        if (_isMousePushed && Input.GetMouseButton(leftMouse)) OnMouseDrag();
        if (_isMousePushed && Input.GetMouseButtonUp(leftMouse))
        {
            _isMousePushed = false;
            OnMouseUp();
            if(_isMouseOver) OnMouseUpAsButton();
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }
    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }
    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
    private void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
    private void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");
    }
    private void OnMouseUpAsButton()
    {
        Debug.Log("OnMouseUpAsButton");
    }
    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }
}