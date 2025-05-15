using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̼� ��ȯ�� �����ϴ� Ŭ����
/// </summary>
public class GoQuickMission : MonoBehaviour
{
    [Header("�̼� ���� ���ӿ�����Ʈ")]
    [Tooltip("�̼� ���� �� Ȱ��ȭ�� ������Ʈ��")]
    public GameObject[] trueObject;

    [Tooltip("�̼� ���� �� ��Ȱ��ȭ�� ������Ʈ��")]
    public GameObject[] falseObject;

    [Header("UI ���")]
    [Tooltip("�̼� ���� ��ư��")]
    public Button[] missionButtons;

    [Header("���� ������Ʈ")]
    public DoTEffect doTEffect;

    // ������ Ȱ��ȭ ���¿��� ������Ʈ���� ���� ����
    private List<GameObject> previouslyActiveObjects = new List<GameObject>();
    private AutoFade autoFade;

    // fade ȿ�� ��� �ð� (fallback)
    private const float DEFAULT_FADE_WAIT_TIME = 0.3f;

    private void Start()
    {
        InitializeComponents();
        SetupMissionButtons();
    }

    /// <summary>
    /// �ʿ��� ������Ʈ�� �ʱ�ȭ
    /// </summary>
    private void InitializeComponents()
    {
        autoFade = FindObjectOfType<AutoFade>();

        // �ʿ��� ������Ʈ�� ���� ��� ��� �α� ���
        if (autoFade == null)
            Debug.LogWarning("AutoFade ������Ʈ�� ã�� �� �����ϴ�.");

        if (doTEffect == null)
            Debug.LogWarning("DoTEffect�� �Ҵ���� �ʾҽ��ϴ�.");
    }

    /// <summary>
    /// �̼� ��ư�� ������ ����
    /// </summary>
    private void SetupMissionButtons()
    {
        if (missionButtons == null)
            return;

        for (int i = 0; i < missionButtons.Length; i++)
        {
            if (missionButtons[i] == null)
                continue;

            int missionIndex = i;
            missionButtons[i].onClick.AddListener(() => { StartMission(missionIndex); });
        }
    }

    /// <summary>
    /// �̼� ���� ó��
    /// </summary>
    /// <param name="missionNum">������ �̼� ��ȣ</param>
    public void StartMission(int missionNum)
    {
        Debug.Log("StartMission called with missionNum: " + missionNum);

        // ����� ����
        StopAudio();

        // Fade ȿ�� ����
        if (autoFade != null)
        {
            autoFade.FadeInOut();
        }

        // DoTEffect �ݱ�
        if (doTEffect != null)
        {
            doTEffect.Close();
        }

        // �̼� ��ȯ �ڷ�ƾ ����
        StartCoroutine(StartMissionCoroutine(missionNum));
    }

    /// <summary>
    /// ����� ���� ó��
    /// </summary>
    private void StopAudio()
    {
        if (Audio_Manager.Instance != null &&
            Audio_Manager.Instance.Ment_audioSource != null)
        {
            Audio_Manager.Instance.Ment_audioSource.Stop();
        }
    }

    /// <summary>
    /// �̼� ��ȯ �ڷ�ƾ
    /// </summary>
    private IEnumerator StartMissionCoroutine(int missionNum)
    {
        // Fade �ð� ���
        float waitTime = (autoFade != null) ?
            autoFade.fadeDurationTime : DEFAULT_FADE_WAIT_TIME;
        yield return new WaitForSeconds(waitTime);

        // Ȱ��ȭ ���¿��� falseObject ���� �� ��Ȱ��ȭ
        SaveAndDeactivateFalseObjects();

        // ������ trueObject Ȱ��ȭ
        ActivateMissionObject(missionNum);
    }

    /// <summary>
    /// falseObject���� ���¸� �����ϰ� ��Ȱ��ȭ
    /// </summary>
    private void SaveAndDeactivateFalseObjects()
    {
        previouslyActiveObjects.Clear();

        if (falseObject == null)
            return;

        foreach (GameObject obj in falseObject)
        {
            if (obj != null && obj.activeSelf)
            {
                previouslyActiveObjects.Add(obj);
                obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ������ �̼� ������Ʈ Ȱ��ȭ
    /// </summary>
    private void ActivateMissionObject(int missionNum)
    {
        if (trueObject == null)
            return;

        if (missionNum < 0 || missionNum >= trueObject.Length)
        {
            Debug.LogWarning($"��ȿ���� ���� �̼� ��ȣ: {missionNum}. ������ 0-{trueObject.Length - 1} �Դϴ�.");
            return;
        }

        GameObject targetObject = trueObject[missionNum];
        if (targetObject != null)
        {
            // ������Ʈ �������ø� ���� ��� ��Ȱ��ȭ�ߴٰ� Ȱ��ȭ
            targetObject.SetActive(false);
            StartCoroutine(DelayedActivation(targetObject));
        }
    }

    /// <summary>
    /// ������Ʈ Ȱ��ȭ ���� ����
    /// </summary>
    private IEnumerator DelayedActivation(GameObject targetObject)
    {
        yield return new WaitForSeconds(0.01f);
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    /// <summary>
    /// ���� ���·� ���� - �̼� ���� �� ȣ��
    /// </summary>
    public void RestorePreviousState()
    {
        // ��� trueObject ��Ȱ��ȭ
        DeactivateAllTrueObjects();

        // ������ Ȱ��ȭ�Ǿ��� falseObject�� �ٽ� Ȱ��ȭ
        RestoreFalseObjects();
    }

    /// <summary>
    /// ��� trueObject ��Ȱ��ȭ
    /// </summary>
    private void DeactivateAllTrueObjects()
    {
        if (trueObject == null)
            return;

        foreach (GameObject obj in trueObject)
        {
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }
    }

    /// <summary>
    /// falseObject���� ���� ���·� ����
    /// </summary>
    private void RestoreFalseObjects()
    {
        foreach (GameObject obj in previouslyActiveObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // ����Ʈ �ʱ�ȭ
        previouslyActiveObjects.Clear();
    }
}