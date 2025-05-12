using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TouchObjectDetector : MonoBehaviour
{
    public LayerMask targetLayer;
    public float rayDistance = 100f;
    public Camera mainCamera;

    public bool isDragging = false;
    public static GameObject selectedObject;
    private Vector3 offset;
    private Vector3 objOriginPos;


    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                Debug.LogError("MainCamera가 없습니다! 카메라를 할당하세요.");
        }

    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#endif
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            DetectObject(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            MoveObject(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;
                DetectObject(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                MoveObject(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                StopDragging();
            }
        }
    }

    private void DetectObject(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, targetLayer == 0 ? -1 : targetLayer))
        {
            selectedObject = hit.collider.gameObject;
            objOriginPos = selectedObject.transform.position;

            TouchSelf touchSelf = selectedObject.GetComponent<TouchSelf>();

            if (touchSelf == null || (touchSelf != null && touchSelf.canDrag))
            {
                isDragging = true;
                Vector3 worldPosition = GetWorldPosition(screenPosition);
                offset = selectedObject.transform.position - worldPosition;
            }
            else
            {
                // 클릭 이벤트만 처리
                if (touchSelf.isBounce)
                {
                    selectedObject.transform.DOScale(selectedObject.transform.localScale * 0.9f, 0.1f).SetLoops(2, LoopType.Yoyo);
                }
                selectedObject.GetComponent<Collider>().enabled = false;
                StartCoroutine(ReenableCollider(selectedObject, 0.3f));
                touchSelf.OnClick();
            }
        }
    }

    private void MoveObject(Vector2 screenPosition)
    {
        if (selectedObject == null) return;
        Vector3 newWorldPosition = GetWorldPosition(screenPosition) + offset;
        selectedObject.transform.position = new Vector3(newWorldPosition.x, newWorldPosition.y, objOriginPos.z);
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, objOriginPos.z));
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    IEnumerator ReenableCollider(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && obj.GetComponent<Collider>() != null)
        {
            obj.GetComponent<Collider>().enabled = true;
        }
    }
}
