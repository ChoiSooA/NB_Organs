using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextActive : MonoBehaviour
{
    public Button nextButton; //���� ��ư
    [Header("Setting Start")]
    public GameObject[] startTrueObject; //�����Ҷ� Ȱ��ȭ�� ������Ʈ
    public GameObject[] startFalseObject; //�����Ҷ� ��Ȱ��ȭ�� ������Ʈ
    public UnityEvent startEvent;
    [Header("Setting Next")]
    public GameObject[] trueObject;
    public GameObject[] falseObject;
    AutoFade autoFade;

    private void OnEnable()
    {
        autoFade = FindObjectOfType<AutoFade>();
        startEvent.Invoke();
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() =>
            {
                autoFade.FadeInOut();
                Next();
            }); ;
        }
        else
        {
            Debug.Log("���� ��ư ����");
        }
        if (startFalseObject.Length == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < startFalseObject.Length; i++)
            {
                startFalseObject[i].SetActive(false);
            }
        }
        if (startTrueObject.Length == 0)
        {
            return;
        }
        else
        {
            for (int i = 0; i < startTrueObject.Length; i++)
            {
                startTrueObject[i].SetActive(true);
            }
        }
    }

    public void Next()
    {
        StartCoroutine(NextCo());
    }

    IEnumerator NextCo()
    {
        yield return new WaitForSeconds(autoFade.fadeDurationTime);  //fadein �ð���ŭ ��ٸ�
        if (trueObject.Length == 0)
        {
            Debug.Log("trueObject�� �������ּ���");
        }
        else
        {
            for (int i = 0; i < trueObject.Length; i++)
            {
                trueObject[i].SetActive(true);
            }
        }
        if (falseObject.Length == 0)
        {
            Debug.Log("falseObject�� �������ּ���");
        }
        else
        {
            for (int i = 0; i < falseObject.Length; i++)
            {
                falseObject[i].SetActive(false);
            }
        }
    }
}
