using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class TouchSelf : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onClickEnd;

    public bool isBounce = true;
    public bool canDrag = false;  // 드래그 여부 제어 필드

    public void OnClick()
    {
        onClick.Invoke();
    }

    public void OnClickEnd()
    {
        onClickEnd.Invoke();
    }
}
