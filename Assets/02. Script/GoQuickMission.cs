using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 미션 전환을 관리하는 클래스
/// </summary>
public class GoQuickMission : MonoBehaviour
{
    [Header("미션 관련 게임오브젝트")]
    [Tooltip("미션 선택 시 활성화될 오브젝트들")]
    public GameObject[] trueObject;

    [Tooltip("미션 선택 시 비활성화될 오브젝트들")]
    public GameObject[] falseObject;

    [Header("UI 요소")]
    [Tooltip("미션 선택 버튼들")]
    public Button[] missionButtons;

    [Header("참조 컴포넌트")]
    public DoTEffect doTEffect;

    // 이전에 활성화 상태였던 오브젝트들의 참조 저장
    private List<GameObject> previouslyActiveObjects = new List<GameObject>();
    private AutoFade autoFade;

    // fade 효과 대기 시간 (fallback)
    private const float DEFAULT_FADE_WAIT_TIME = 0.3f;

    private void Start()
    {
        InitializeComponents();
        SetupMissionButtons();
    }

    /// <summary>
    /// 필요한 컴포넌트들 초기화
    /// </summary>
    private void InitializeComponents()
    {
        autoFade = FindObjectOfType<AutoFade>();

        // 필요한 컴포넌트가 없는 경우 경고 로그 출력
        if (autoFade == null)
            Debug.LogWarning("AutoFade 컴포넌트를 찾을 수 없습니다.");

        if (doTEffect == null)
            Debug.LogWarning("DoTEffect가 할당되지 않았습니다.");
    }

    /// <summary>
    /// 미션 버튼에 리스너 설정
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
    /// 미션 시작 처리
    /// </summary>
    /// <param name="missionNum">시작할 미션 번호</param>
    public void StartMission(int missionNum)
    {
        Debug.Log("StartMission called with missionNum: " + missionNum);

        // 오디오 정지
        StopAudio();

        // Fade 효과 시작
        if (autoFade != null)
        {
            autoFade.FadeInOut();
        }

        // DoTEffect 닫기
        if (doTEffect != null)
        {
            doTEffect.Close();
        }

        // 미션 전환 코루틴 시작
        StartCoroutine(StartMissionCoroutine(missionNum));
    }

    /// <summary>
    /// 오디오 정지 처리
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
    /// 미션 전환 코루틴
    /// </summary>
    private IEnumerator StartMissionCoroutine(int missionNum)
    {
        // Fade 시간 대기
        float waitTime = (autoFade != null) ?
            autoFade.fadeDurationTime : DEFAULT_FADE_WAIT_TIME;
        yield return new WaitForSeconds(waitTime);

        // 활성화 상태였던 falseObject 저장 및 비활성화
        SaveAndDeactivateFalseObjects();

        // 선택한 trueObject 활성화
        ActivateMissionObject(missionNum);
    }

    /// <summary>
    /// falseObject들의 상태를 저장하고 비활성화
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
    /// 선택한 미션 오브젝트 활성화
    /// </summary>
    private void ActivateMissionObject(int missionNum)
    {
        if (trueObject == null)
            return;

        if (missionNum < 0 || missionNum >= trueObject.Length)
        {
            Debug.LogWarning($"유효하지 않은 미션 번호: {missionNum}. 범위는 0-{trueObject.Length - 1} 입니다.");
            return;
        }

        GameObject targetObject = trueObject[missionNum];
        if (targetObject != null)
        {
            // 오브젝트 리프레시를 위해 잠시 비활성화했다가 활성화
            targetObject.SetActive(false);
            StartCoroutine(DelayedActivation(targetObject));
        }
    }

    /// <summary>
    /// 오브젝트 활성화 지연 실행
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
    /// 이전 상태로 복원 - 미션 종료 시 호출
    /// </summary>
    public void RestorePreviousState()
    {
        // 모든 trueObject 비활성화
        DeactivateAllTrueObjects();

        // 이전에 활성화되었던 falseObject들 다시 활성화
        RestoreFalseObjects();
    }

    /// <summary>
    /// 모든 trueObject 비활성화
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
    /// falseObject들을 이전 상태로 복원
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

        // 리스트 초기화
        previouslyActiveObjects.Clear();
    }
}