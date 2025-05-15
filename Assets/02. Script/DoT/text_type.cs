using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class text_type : MonoBehaviour
{
    [System.Serializable]
    public class MentAnimPair
    {
        [TextArea(2, 5)]
        public string mentText;  // 여러 줄 입력 가능
        public string animationName;
        public AudioClip mentClip;
        public bool withTextEffect; // 특정 문장에 텍스트 애니메이션 적용 여부
    }

    [SerializeField] private UnityEvent event_finish;
    [SerializeField] private bool withJump = true; // 전체 점프 실행 여부
    [SerializeField] private float waitAfterText = 0.6f; // 텍스트 후 대기 시간
    [SerializeField] private float initialDelay = 1f; // 초기 대기 시간

    private TMP_Text outMent;
    public MentAnimPair[] mentAnimPairs;
    public float typingSpeed = 0.1f;

    private bool isPlaying = false;
    private Coroutine textCoroutine;
    private Sequence scaleSequence;

    private void Awake()
    {
        outMent = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        ResetText();

        // 초기 스케일 애니메이션
        if (scaleSequence != null)
        {
            scaleSequence.Kill();
        }
        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.parent.DOScale(transform.localScale * 0.95f, 0.2f).SetLoops(4, LoopType.Yoyo));

        // 텍스트 타이핑 시작
        if (mentAnimPairs != null && mentAnimPairs.Length > 0)
        {
            textCoroutine = StartCoroutine(NextText());
        }
    }

    private void OnDisable()
    {
        CleanUp();
    }

    private void ResetText()
    {
        outMent.text = "";
    }

    private void CleanUp()
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
            textCoroutine = null;
        }

        if (scaleSequence != null)
        {
            scaleSequence.Kill();
            scaleSequence = null;
        }

        AnimationManager.Instance.ResetVector();
        outMent.DOKill();
        ResetText();
        isPlaying = false;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    IEnumerator NextText()
    {
        isPlaying = true;

        yield return new WaitForSeconds(initialDelay);

        // 전체 시작 시 점프 여부 체크
        if (withJump)
        {
            AnimationManager.Instance.jump();
            yield return new WaitForSeconds(0.8f);
        }

        for (int i = 0; i < mentAnimPairs.Length; i++)
        {
            var current = mentAnimPairs[i];
            ResetText();

            // 애니메이션 실행 (텍스트와 동시에)
            if (!string.IsNullOrEmpty(current.animationName))
            {
                AnimationManager.Instance.PlayAnimation(current.animationName);
                yield return new WaitForSeconds(0.3f);
            }

            // 텍스트 & 음성 출력
            float textDuration;

            if (current.mentClip != null)
            {
                // 음성 클립이 있는 경우, 클립 길이를 기준으로 타이핑 속도 결정
                textDuration = current.mentClip.length;
                Audio_Manager.Instance.PlayMent(current.mentClip);
            }
            else
            {
                // 음성 클립이 없는 경우, 텍스트 길이 * 타이핑 속도로 계산
                textDuration = Mathf.Max(1f, current.mentText.Length * typingSpeed);
            }

            // 텍스트 타이핑 실행
            if (!string.IsNullOrEmpty(current.mentText))
            {
                Tween textTween = outMent.DOText(current.mentText, textDuration).SetEase(Ease.Linear);

                yield return textTween.WaitForCompletion();

                // 텍스트 애니메이션은 텍스트가 모두 표시된 후에 실행
                if (current.withTextEffect)
                {
                    outMent.transform.DOScale(1.15f, 0.3f).SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo);
                    yield return new WaitForSeconds(0.6f); // 애니메이션 시간만큼 대기
                }

                yield return new WaitForSeconds(waitAfterText);
            }
        }

        event_finish?.Invoke();
        isPlaying = false;
    }

    // 외부에서 현재 실행 중인 텍스트 타이핑을 스킵하는 기능
    public void SkipCurrentText()
    {
        if (!isPlaying) return;

        // 현재 진행 중인 DOTween 애니메이션 완료
        outMent.DOComplete();

        // 현재 코루틴 중지 및 이벤트 발생
        CleanUp();
        event_finish?.Invoke();
    }
}