using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoQuickMission : MonoBehaviour
{
    public GameObject[] trueObject;
    public GameObject[] falseObject;
    public Button[] BT_Mission;

    public DoTEffect doTEffect;

    private List<GameObject> previouslyActiveObjects = new List<GameObject>();
    private AutoFade autoFade;

    private void Start()
    {
        autoFade = FindObjectOfType<AutoFade>();

        for (int i = 0; i < BT_Mission.Length; i++)
        {
            int missionIndex = i;
            BT_Mission[i].onClick.AddListener(() => { GoMission(missionIndex); });
        }
    }

    public void GoMission(int missionNum)
    {
        Debug.Log("GoMission called with missionNum: " + missionNum);
        Audio_Manager.Instance.Ment_audioSource.Stop();
        if (autoFade != null)
        {
            autoFade.FadeInOut();
        }

        doTEffect.Close();

        StartCoroutine(GoMissionCo(missionNum));
    }

    IEnumerator GoMissionCo(int missionNum)
    {
        if (autoFade != null)
            yield return new WaitForSeconds(autoFade.fadeDurationTime);
        else
            yield return new WaitForSeconds(0.3f); // fallback

        // 이전에 켜져 있던 falseObject 저장
        previouslyActiveObjects.Clear();
        foreach (GameObject obj in falseObject)
        {
            if (obj != null && obj.activeSelf)
            {
                previouslyActiveObjects.Add(obj);
                obj.SetActive(false);
            }
        }

        // trueObject[missionNum] 껐다가 다시 켜기
        if (missionNum >= 0 && missionNum < trueObject.Length && trueObject[missionNum] != null)
        {
            trueObject[missionNum].SetActive(false);
            yield return new WaitForSeconds(0.01f);
            trueObject[missionNum].SetActive(true);
        }
    }

    public void RestorePreviousState()
    {
        // 현재 trueObject 꺼줌
        foreach (GameObject obj in trueObject)
        {
            if (obj != null && obj.activeSelf)
                obj.SetActive(false);
        }

        // 이전 falseObject 중 켜졌던 것만 다시 활성화
        foreach (GameObject obj in previouslyActiveObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        previouslyActiveObjects.Clear();
    }
}
