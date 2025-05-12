using UnityEngine;
using UnityEngine.EventSystems;

public class RotateModel : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("회전 설정")]
    public RotationAxis rotateAxis = RotationAxis.Y;
    [SerializeField] private float rotationSensitivity = 1.0f;

    private Vector3 prevTouchPosition;
    private bool isDragging = false;

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#endif
        HandleTouchInput();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsOverUI())
        {
            isDragging = true;
            prevTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging && !IsOverUI())
        {
            Vector3 delta = Input.mousePosition - prevTouchPosition;
            RotateByAxis(delta);
            prevTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && !IsOverUI(touch.fingerId))
            {
                isDragging = true;
                prevTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging && !IsOverUI(touch.fingerId))
            {
                Vector3 delta = (Vector3)touch.position - prevTouchPosition;
                RotateByAxis(delta);
                prevTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    void RotateByAxis(Vector3 delta)
    {
        float rotationAmount = delta.x * rotationSensitivity;

        switch (rotateAxis)
        {
            case RotationAxis.X:
                transform.Rotate(Vector3.right, rotationAmount, Space.World);
                break;
            case RotationAxis.Y:
                transform.Rotate(Vector3.up, -rotationAmount, Space.World);
                break;
            case RotationAxis.Z:
                transform.Rotate(Vector3.forward, rotationAmount, Space.World);
                break;
        }
    }

    bool IsOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    bool IsOverUI(int fingerId)
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(fingerId);
    }
}
