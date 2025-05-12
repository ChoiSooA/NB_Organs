/*
 * ������� ���� ������� �����־���, �Ǻ��� ����� ������ �� ��ġ�� �����ؾ���
 * �ٿ��� ����� ���� ��ġ�� �ƴ� ������ ��ġ�� �����ؾ���
 * ���Ĵ� �⺻������ 0���� �����صξ����(��Ÿ���°� ����, ������� �� ��)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DoTEffect : MonoBehaviour
{
    public enum State
    {
        Size,
        Bound,
        Move,
        Alpha,
        Blink,
    }
    public State state;                 //������, �ٿ��, ����, ���� �� ����� ȿ��
    public float durationTime = 1f;     //���ӽð�
    public Vector3 targetVector;        //Ÿ�� ����(������, ��ġ ��)
    public GameObject falseObject;        //��Ȱ��ȭ ������Ʈ
    Vector3 OriginalSize;               //���� ������
    Vector3 OriginalPosition;           //���� ��ġ

    private void Awake()
    {
        OriginalSize = transform.localScale;
        OriginalPosition = transform.position;
    }
    private void OnEnable()
    {
        StartDoT();
    }
    public void Close()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("[DoTEffect] Close() ȣ�� �� GameObject�� ��Ȱ��ȭ �����Դϴ�.");
            return;
        }
        EndDoT();
    }

    public void StartDoT()
    {
        switch (state)
        {
            case State.Size:
                SizeStart();
                break;
            case State.Bound:
                BoundStart();
                break;
            case State.Move:
                MoveStart();
                break;
            case State.Alpha:
                AlphaStart();
                break;
            case State.Blink:
                BlinkStart();
                break;
        }
    }
    public void EndDoT()
    {
        switch (state)
        {
            case State.Size:
                StartCoroutine(SizeEnd());
                break;
            case State.Alpha:
                StartCoroutine(AlphaEnd());
                break;
            default:
                StartCoroutine(OriginalEnd());
                break;
        }
    }

    void SizeStart()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(targetVector, durationTime);
    }
    void BoundStart()
    {
        transform.DOLocalMove(targetVector, durationTime).SetEase(Ease.OutBounce);
    }
    void MoveStart()
    {
        transform.DOLocalMove(targetVector, durationTime);
    }
    void AlphaStart()
    {
        transform.GetComponent<UnityEngine.UI.Image>().DOFade(1, durationTime);
    }
    void BlinkStart()
    {
        transform.GetComponent<TextMeshProUGUI>().DOFade(0f, durationTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }
    IEnumerator SizeEnd()
    {
        transform.DOScale(Vector3.zero, durationTime);
        yield return new WaitForSeconds(durationTime * 0.8f);
        transform.localScale = Vector3.zero;
        if (falseObject != null)
            falseObject.SetActive(false);
        else
            gameObject.SetActive(false);
        yield return null;
    }
    IEnumerator OriginalEnd()
    {
        transform.DOMove(OriginalPosition, durationTime);
        yield return new WaitForSeconds(durationTime * 0.8f);
        transform.position = OriginalPosition;
        yield return null;
    }
    IEnumerator AlphaEnd()
    {
        transform.GetComponent<UnityEngine.UI.Image>().DOFade(0, durationTime);
        yield return new WaitForSeconds(durationTime);
        yield return null;
    }
}
