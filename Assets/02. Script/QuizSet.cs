using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class QuizSet : MonoBehaviour
{
    [System.Serializable]
    public struct Quiz
    {
        public string answer;
        public string question;
        public AudioClip audioClip;
    }

    [Header("퀴즈 관련")]
    public List<Quiz> quizList = new List<Quiz>();
    public TMP_Text questionText;

    [Header("결과 관련")]
    public AudioClip resultAudioClip;
    public GameObject finishPanel;
    public TMP_Text resultText;
    public Button resultButton;
    public Button retryButton;

    [Header("정답 후보")]
    public GameObject[] answerObjects;

    int currentQuiz = 0;
    int correctCount = 0;
    GameObject zoomObj;
    List<string> summaryLog = new List<string>();

    private void OnEnable()
    {
        StartQuiz();
    }

    private void OnDisable()
    {
        ResetState();
    }

    void StartQuiz()
    {
        ResetState();
        MixQuiz();
        ShowQuiz();

        resultButton.onClick.RemoveAllListeners();
        resultButton.onClick.AddListener(ShowFinishPanel);
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(StartQuiz);
    }

    void ResetState()
    {
        currentQuiz = 0;
        correctCount = 0;
        summaryLog.Clear();
        finishPanel.SetActive(false);
        resultButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        questionText.text = "";
        questionText.transform.localScale = Vector3.one;

        foreach (var obj in answerObjects)
        {
            obj.GetComponent<Collider>().enabled = true;
            obj.SetActive(true);
        }
    }

    void MixQuiz()
    {
        for (int i = quizList.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            var temp = quizList[i];
            quizList[i] = quizList[rnd];
            quizList[rnd] = temp;
        }
    }

    void ShowQuiz()
    {
        if (currentQuiz < quizList.Count)
        {
            Quiz quiz = quizList[currentQuiz];

            questionText.transform.localScale = Vector3.zero;
            questionText.transform
                .DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .OnStart(() =>
                {
                    questionText.text = quiz.question;
                    if (quiz.audioClip != null)
                        Audio_Manager.Instance.PlayMent(quiz.audioClip);
                })
                .OnComplete(() =>
                {
                    foreach (var obj in answerObjects)
                        obj.GetComponent<Collider>().enabled = true;
                });
        }
        else
        {
            questionText.transform.localScale = Vector3.zero;
            questionText.transform
                .DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)
                .OnStart(() =>
                {
                    questionText.text = "<size=130>모든 문제를 풀었어요!</size>\n아래 정답확인 버튼을 눌러 점수를 확인해봐요!";
                });

            Audio_Manager.Instance.PlayMent(resultAudioClip);
            resultButton.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(true);

            foreach (var obj in answerObjects)
                obj.SetActive(false);
        }
    }

    public void CheckQuiz()
    {
        questionText.transform.DOScale(Vector3.zero, 0.3f);

        zoomObj = TouchObjectDetector.selectedObject;
        if (zoomObj == null) return;

        Quiz quiz = quizList[currentQuiz];
        bool isCorrect = (zoomObj.name == quiz.answer);

        if (isCorrect)
        {
            correctCount++;
            Audio_Manager.Instance.PlayEffect(4);
            Audio_Manager.Instance.PlayEffect(3);
            Audio_Manager.Instance.Ment_audioSource.Stop();

            summaryLog.Add($"<color=#65D169>{quiz.question} → 정답</color>\n");
        }
        else
        {
            Audio_Manager.Instance.PlayEffect(2);
            Audio_Manager.Instance.Ment_audioSource.Stop();
            summaryLog.Add($"<color=#F44336>{quiz.question} → 오답</color>\n");
        }

        currentQuiz++;

        foreach (var obj in answerObjects)
            obj.GetComponent<Collider>().enabled = false;

        StartCoroutine(NextQuizWithDelay());
    }

    IEnumerator NextQuizWithDelay()
    {
        yield return new WaitForSeconds(0.6f);
        ShowQuiz();
    }

    public void ShowFinishPanel()
    {
        finishPanel.SetActive(true);
        string scoreLine = $"<size=140><color=#FFD700>{correctCount} / {quizList.Count} 정답</color></size>\n";

        string summary = "<size=65><line-height=120%>\n";
        foreach (var line in summaryLog)
            summary += line + "\n";
        summary += "</line-height></size>";

        resultText.text = scoreLine + summary;
    }
}