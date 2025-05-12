using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class TextTyping : MonoBehaviour
{
    [System.Serializable]
    public class MentAnimPair
    {
        [TextArea(2, 5)]
        public string mentText;  // 여러 줄 입력 가능
        public string animationName;
        public AudioClip mentClip;
    }

    [SerializeField] private UnityEvent event_finish;
    [SerializeField] private bool withJump = true; // 전체 점프 실행 여부

    TMP_Text outMent;
    public MentAnimPair[] mentAnimPairs;

    public float typingSpeed = 0.1f;
    bool coroutine_running = false;

    private void Awake()
    {
        outMent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        outMent.text = "";
        transform.parent.DOScale(transform.localScale * 0.95f, 0.2f).SetLoops(4, LoopType.Yoyo);

        if (mentAnimPairs != null && mentAnimPairs.Length > 0)
        {
            StartCoroutine(NextText());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        // 안전한 호출 조건
        if (AnimationManager.Instance != null && AnimationManager.Instance.Character != null)
        {
            AnimationManager.Instance.ResetVector();
        }

        outMent.DOKill();
        outMent.text = "";
    }

    IEnumerator NextText()
    {
        yield return new WaitForSeconds(1f);
        coroutine_running = true;

        // 전체 시작 시 점프 여부 체크
        if (withJump)
        {
            AnimationManager.Instance.jump();
            yield return new WaitForSeconds(0.8f);
        }

        foreach (var current in mentAnimPairs)
        {
            outMent.text = "";

            // 애니메이션 실행 (텍스트와 동시에)
            if (!string.IsNullOrEmpty(current.animationName))
            {
                AnimationManager.Instance.PlayAnimation(current.animationName);
                yield return new WaitForSeconds(0.3f);
            }

            // 텍스트 & 음성 출력
            if (current.mentClip != null)
            {
                float textSpeed = current.mentClip.length;
                outMent.DOText(current.mentText, textSpeed).SetEase(Ease.Linear);
                Audio_Manager.Instance.PlayMent(current.mentClip);
                yield return new WaitForSeconds(textSpeed + 0.6f);
            }
            else if (!string.IsNullOrEmpty(current.mentText))
            {
                float nextSpeed = current.mentText.Length * typingSpeed;
                outMent.DOText(current.mentText, nextSpeed).SetEase(Ease.Linear);
                yield return new WaitForSeconds(nextSpeed + 0.6f);
            }
        }

        event_finish?.Invoke();
        coroutine_running = false;
        yield return new WaitForSeconds(0.3f);
    }
}
