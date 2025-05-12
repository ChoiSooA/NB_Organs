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
        public string mentText;  // ���� �� �Է� ����
        public string animationName;
        public AudioClip mentClip;
    }

    [SerializeField] private UnityEvent event_finish;
    [SerializeField] private bool withJump = true; // ��ü ���� ���� ����

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

        // ������ ȣ�� ����
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

        // ��ü ���� �� ���� ���� üũ
        if (withJump)
        {
            AnimationManager.Instance.jump();
            yield return new WaitForSeconds(0.8f);
        }

        foreach (var current in mentAnimPairs)
        {
            outMent.text = "";

            // �ִϸ��̼� ���� (�ؽ�Ʈ�� ���ÿ�)
            if (!string.IsNullOrEmpty(current.animationName))
            {
                AnimationManager.Instance.PlayAnimation(current.animationName);
                yield return new WaitForSeconds(0.3f);
            }

            // �ؽ�Ʈ & ���� ���
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
