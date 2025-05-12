using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AnimationManager : MonoBehaviour
{
    public GameObject Character;
    public Animator animator;

    public Vector3 resetPosition;
    public Vector3 resetRotation;
    public Vector3 resetScale;

    Coroutine currentCoroutine;

    public static AnimationManager Instance { get; private set; }

    private void Awake()
    {
        /*resetPosition = Character.transform.localPosition;
        resetRotation = Character.transform.localRotation.eulerAngles;
        resetScale = Character.transform.localScale;*/
        animator = Character.GetComponent<Animator>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        /*resetPosition = Character.transform.localPosition;
        resetRotation = Character.transform.localRotation.eulerAngles;
        resetScale = Character.transform.localScale;*/
    }

    public void ResetVector()
    {
        if (Character == null) return;

        Character.transform.localPosition = resetPosition;
        Character.transform.localRotation = Quaternion.Euler(resetRotation);
        Character.transform.localScale = resetScale;
    }
    public void ResetSideVector()
    {
        if (Character == null) return;
        Character.transform.localPosition = new Vector3(1, -5, 0);
        Character.transform.localRotation = Quaternion.Euler(resetRotation);
        Character.transform.localScale = resetScale;
    }
    private void OnDisable()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // PlayAnimation("Idle");  �����ϰų� �Ʒ�ó�� ��ȣ
        if (gameObject.activeInHierarchy)
            PlayAnimation("Idle");

        ResetVector();
    }

    public void PlayAnimation(string animationName)
    {
        if (string.IsNullOrEmpty(animationName)) return;

        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }

        currentCoroutine = StartCoroutine(PlayTriggerAndReturnToIdle(animationName));
    }

    private IEnumerator PlayTriggerAndReturnToIdle(string triggerName)
    {
        yield return null;
        animator.SetTrigger(triggerName);

        // ���� ���� ���� ���
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName(triggerName));

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        // �ڵ����� Idle ����
        animator.SetTrigger("Idle");
    }

    public void jump()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;

        PlayAnimation("Jump");
        Character.transform.DOMoveY(-2f, 0.7f).SetEase(Ease.Linear);
    }

    public void FinishAnim()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(FinAnim());
    }

    IEnumerator FinAnim()
    {
        ResetSideVector();
        yield return new WaitForSeconds(0.01f);
        jump();
        yield return new WaitForSeconds(1f);
        PlayAnimation("Clap");
        Audio_Manager.Instance.PlayEffect(3);
        yield return new WaitForSeconds(2.2f);
        PlayAnimation("Nice");
        yield return new WaitForSeconds(10f);    //������ clap ������ ��� �ð�
        PlayAnimation("Hi");
    }
}
