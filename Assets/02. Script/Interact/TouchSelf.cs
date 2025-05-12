using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class TouchSelf : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private UnityEvent onClickEnd;

    public bool isBounce = true;
    public bool canDrag = false;  // �巡�� ���� ���� �ʵ�

    public void OnClick()
    {
        onClick.Invoke();
    }

    public void OnClickEnd()
    {
        onClickEnd.Invoke();
    }
}
